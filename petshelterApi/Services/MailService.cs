using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using petshelterApi.Database;
using petshelterApi.Domain;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace petshelterApi.Services
{
    // https://stackoverflow.com/questions/38840101/how-to-send-a-ical-invite-with-mailgun-rest-api-c

    public class MailService
    {
        private readonly IConfiguration _configuration;
        private readonly PetShelterDbContext _petShelterDbContext;

        private UserProfile _userProfile;
        private Appointment _appointment;

        public MailService(IConfiguration configuration, PetShelterDbContext petShelterDbContext)
        {
            _configuration = configuration;
            _petShelterDbContext = petShelterDbContext;
        }

        public async Task<IRestResponse> SendAppointmentMail(object appointmentId, UserProfile userProfile)
        {
            if (userProfile == null || !int.TryParse(appointmentId.ToString(), out var id))
            {
                throw new InvalidOperationException(); //..???
            }

            _userProfile = userProfile;
            _appointment = _petShelterDbContext.Appointments
                .Include(x => x.Pet.Shelter)                
                .FirstOrDefault(x => x.Id == id);

            RestClient client = new RestClient
            {
                BaseUrl = new Uri(_configuration["Mailgun:Audience"]),
                Authenticator = new HttpBasicAuthenticator("api", _configuration["Mailgun:Token"])
            };

            var request = new RestRequest();
            request.AddParameter("domain", _configuration["Mailgun:Domain"], ParameterType.UrlSegment);
            request.Resource = "{domain}/messages.mime";
            request.AddParameter("to", _configuration["Mailgun:TestMail"]);
            var mail = new AppointmentMail()
            {
                To = _configuration["Mailgun:TestMail"],
                From = _configuration["Mailgun:From"],
                Subject = $"Je afspraak met {_appointment.Pet.Name}!",
                Text = string.Join(Environment.NewLine, GetPlainText()),
                TextHtml = string.Concat(GetHtmlBody(GetPlainText())),
                Start = _appointment.Start,
                PetName = _appointment.Pet.Name,
                ICalMessage = GetIcalMessage()
            };
            request.AddFile(
                "message",
                Encoding.UTF8.GetBytes(GetMailWithICalInvite(mail)),
                "message.mime");
            request.Method = Method.POST;

            var cancellationTokenSource = new CancellationTokenSource();
            return await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
        }

        private string GetMailWithICalInvite(AppointmentMail message)
        {
            var textBody = new TextPart("plain") { Text = message.Text };
            var htmlBody = new TextPart("html") { Text = message.TextHtml };

            // add views to the multipart/alternative
            var alternative = new Multipart("alternative")
            {
                textBody,
                htmlBody
            };

            if (!string.IsNullOrWhiteSpace(message.ICalMessage))
            {
                // also add the calendar as an alternative view
                // encoded as base64, but 7bit will also work
                var calendarBody = new TextPart("calendar")
                {
                    Text = message.ICalMessage,
                    ContentTransferEncoding = ContentEncoding.Base64
                };

                // most clients wont recognise the alternative view without the 
                // method=REQUEST header
                calendarBody.ContentType.Parameters.Add("method", "REQUEST");
                alternative.Add(calendarBody);
            }

            // create the multipart/mixed that will contain the multipart/alternative
            // and all attachments
            var multiPart = new Multipart("mixed") { alternative };
            if (!string.IsNullOrWhiteSpace(message.ICalMessage))
            {

                // add the calendar as an attachment
                var calAttachment = new MimePart("application", "ics")
                {
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = "invite.ics",
                    Content = new MimeContent(GenerateStreamFromString(message.ICalMessage))
                };

                multiPart.Add(calAttachment);
            }

            // TODO: Add any other attachements to 'multipart' here.

            // build final mime message
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("petshelter", message.From));
            mimeMessage.To.Add(new MailboxAddress(
                _userProfile.Email.Substring(0, _userProfile.Email.IndexOf('@')), 
                _configuration["Mailgun:TestMail"]));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = multiPart;

            // parse and return mime message
            return mimeMessage.ToString();

        }

        public class AppointmentMail
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Subject { get; set; }
            public string Text { get; set; }
            public string TextHtml { get; set; }
            public string PetName { get; set; }
            public string ShelterName { get; set; }
            public DateTimeOffset Start { get; set; }
            public string ICalMessage { get; set; }
        }

        private string[] GetPlainText()
        {
            return new[]
            {
                $"Hallo {_userProfile.Email.Substring(0, _userProfile.Email.IndexOf('@'))}",
                $"Je hebt een afspraak op {_appointment.Start.LocalDateTime:f},",
                $"in {_appointment.Pet.Shelter.Name},",
                $"{_appointment.Pet.Shelter.Address.LineOne},",
                $"{_appointment.Pet.Shelter.Address.LineThree}.",
                "Als je agenda gekoppeld is aan je email, kan je hierboven je afspraak toevoegen.",
                "Tot binnenkort!",
                "Met vriendelijke groeten",
                $"{_appointment.Pet.Shelter.Name}"
            };
        }

        public IEnumerable<string> GetHtmlBody(string[] plaintext)
        {
            yield return "<body>";
            yield return $"<h2>{plaintext[0]}</h2>";
            for (var i = 1; i < plaintext.Length; i++)
            {
                yield return $"<div>{plaintext[i]}</div>";
            }
            yield return "</body>";
        }

        public string GetIcalMessage()
        {
            var e = new CalendarEvent
            {
                Start = new CalDateTime(_appointment.Start.LocalDateTime),
                End = new CalDateTime(_appointment.Start.AddHours(1).LocalDateTime),
                Organizer = new Organizer(_configuration["Mailgun:From"]),
                Summary = $"Je afspraak met {_appointment.Pet.Name}",
                Location = $"{_appointment.Pet.Shelter.Name}, {_appointment.Pet.Shelter.Address.LineOne}, {_appointment.Pet.Shelter.Address.LineThree}"
            };
            var calendar = new Calendar();
            calendar.Events.Add(e);
            var serializer = new CalendarSerializer();
            var icalString = serializer.SerializeToString(calendar);
            return icalString;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
