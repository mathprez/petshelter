<template>
    <section class="hero is-white is-medium">
        <div class="hero-body">
            <div class="container">
                <div class="columns">
                    <div class="column">
                        <div class="title">Maak een afspraak met {{petName}}</div>
                    </div>
                </div>
                <div class="columns">
                    <div class="column center-field-column">
                        <label class="label align-label-left">
                            Selecteer een datum
                        </label>
                        <b-datepicker :min-date="minDate"
                                      :month-names="monthNames"
                                      :day-names="dayNames"
                                      :first-day-of-week="firstDayOfWeek"
                                      v-model="selectedDate"
                                      inline
                                      placeholder="Klik om te selecteren..."
                                      icon="calendar"
                                      icon-pack="fas">
                        </b-datepicker>
                    </div>
                    <div class="column is-two-thirds-fullhd">
                        <div class="appt-timepicker-container">
                            <label class="label">
                                Selecteer een tijd
                            </label>
                            <b-button size="is-medium"
                                      icon-left="clock"
                                      icon-pack="fas"
                                      v-for="slot in slots"
                                      @click="setTime(slot.start)"
                                      class="timeslot"
                                      v-bind:class="{'is-danger': !slot.available, 'is-grey': slot.available }"
                                      :disabled="!slot.available"
                                      :label="slot.start"
                                      :outlined="!isSelected(slot)">
                                <span class="icon is-small"><i class="fas fa-clock fa-1x"></i></span>
                                {{slot.start}}
                            </b-button>
                        </div>
                    </div>
                </div>
                <div class="columns">
                    <div class="column">
                        <b-button @click="postAppointment()" size="is-large" class="is-primary is-fullwidth">Maak afspraak!</b-button>
                    </div>
                </div>
            </div>

        </div>
    </section>
</template>

<script>
    import PetCard from '../components/PetCard.vue'
    export default {
        name: 'CreateAppointment',
        data() {
            const today = new Date();
            return {
                petId: this.$route.params.petId,
                availableDayTemplate: null,
                appointmentDays: null,
                slots: [],
                selectedDate: null,
                selectedTime: null,
                petName: null,
                minDate: new Date(today.getFullYear(), today.getMonth(), today.getDate() + 1),
                maxDate: new Date(today.getFullYear() + 2, today.getMonth(), today.getDate()),
                monthNames: ['Januari', 'Februari', 'Maart', 'April', 'Mei', 'Juni', 'Juli', 'Augustus', 'September', 'Oktober', 'November', 'December'],
                dayNames: ['Zon', 'Ma', 'Din', 'Woe', 'Don', 'Vrij', 'Zat'],
                firstDayOfWeek: 1
            }
        },
        components: {
            PetCard
        },
        mounted() {
            this.getAppointmentInfo();
        },
        methods: {
            async getAppointmentInfo() {
                try {
                    const accessToken = await this.$auth.getAccessToken();
                    const { data } = await this.$http.get(`${this.$auth.audience}/api/appointments/create/${this.petId}`, {
                        headers: {
                            Authorization: `Bearer ${accessToken}`
                        }
                    });

                    this.petName = data.payload.pet.name;
                    this.availableDayTemplate = data.payload.availableDayTemplate;
                    this.appointmentDays = data.payload.appointmentDays;
                    var now = new Date();
                    var tomorrow = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1);
                    this.selectedDate = tomorrow;
                }
                catch (e) {
                    window.console.log(e);
                    this.$snackbar.open({
                        message: 'Er is iets foutgelopen. Probeer het later opnieuw.',
                        type: 'is-danger',
                        position: 'is-top',
                        actionText: 'OK'
                    })
                }
            },
            getSlots() {
                var vm = this;
                vm.slots = vm.availableDayTemplate.slots;
                this.appointmentDays.forEach((element, index) => {
                    var date = new Date();
                    date.setTime(Date.parse(element.date));
                    if (date.getTime() == vm.selectedDate.getTime()) {
                        vm.slots = element.slots;
                    }
                });
            },
            setTime(start) {
                this.selectedTime = start;
            },
            isSelected(slot) {
                return slot.available && this.selectedTime === slot.start;
            },
            async postAppointment() {
                if (!this.selectedDate || !this.selectedTime) {
                    this.$snackbar.open({
                        message: 'Selecteer een tijd!',
                        type: 'is-warning',
                        position: 'is-bottom'
                    });
                    return;
                }

                if (!this.selectedDateTime || !this.selectedDateTime.getMonth()) {
                    this.$snackbar.open({
                        message: 'Er is een fout in de JS code postAppointment()!',
                        type: 'is-warning',
                        position: 'is-bottom'
                    });
                    return;
                }

                console.log(this.selectedDateTime);

                const accessToken = await this.$auth.getAccessToken();
                let response = await this.$http.post(`${this.$auth.audience}/api/appointments/create`,
                    {
                        petId: this.petId,
                        date: this.selectedDateTime,
                    }, {

                        headers: {
                            Authorization: `Bearer ${accessToken}`
                        }

                    });

                if (response.status == 200) {
                    this.$router.push('/appointments')
                    this.$snackbar.open({
                        message: 'Afspraak gemaakt!',
                        type: 'is-success',
                        position: 'is-bottom'
                    });
                }
                else {
                    this.$snackbar.open({
                        message: 'Er liep iets fout aan serverkant.',
                        type: 'is-warning',
                        position: 'is-bottom'
                    });
                }
            }
        },
        computed: {
            selectedDateTime() {
                if (!this.selectedDate || !this.selectedTime) {
                    return null;
                }
                var combined = this.selectedDate;
                var time = this.selectedTime.split(':');
                combined.setHours(time[0]);
                combined.setMinutes(time[1]);
                combined.setSeconds(time[2]);
                console.log(combined);
                return combined;
            }
        },
        watch: {
            selectedDate() {
                this.getSlots();
                this.selectedTime = null;
            },
            selectedTime() {
                var removethis = this.selectedDateTime;
            }
        }
    }
</script>

<style lang="scss" scoped>
    @import "../styles/appointment.scss";
</style>