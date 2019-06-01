<template>
    <div v-if="isAuthenticated">
        <link href="https://cdn.jsdelivr.net/npm/animate.css@3.5.1" rel="stylesheet" type="text/css">
        <section class="hero is-white is-medium ">
            <div class="hero-body">
                <div class="container">
                    <div v-if="appointments && appointments.length">

                        <h1 class="title">Dit zijn je afspraken, {{profile.nickname}}!</h1>
                        <div class="appointment-container">
                            <div v-for="appointment in appointments" v-bind:key="appointment.id">
                                <transition name="custom-classes-transition"
                                            leave-active-class="animated bounceOutRight">
                                    <div v-if="!appointment.isCancelled" class="appointment-card">
                                        <div class="appointment-element">
                                            <h2 class="title">{{appointment.pet.name}}</h2>
                                        </div>
                                        <div class="appointment-element has-image">
                                            <figure>
                                                <img class="is-pet-image" :src="appointment.pet.externalImage || appointment.pet.base64Image" />
                                            </figure>
                                        </div>
                                        <div class="appointment-element">
                                            <p class="subtitle"><i class="fas fa-map-marker-alt" style="margin-right:8px"></i></p>
                                            <p v-for="addressline in appointment.shelterAddress">{{addressline}}</p>
                                        </div>
                                        <div class="appointment-element">
                                            <p class="subtitle"><i class="fas fa-clock" style="margin-right:8px"></i></p>
                                            <p>{{appointment.date}} {{appointment.year}}</p>
                                            <p>van {{appointment.start}} tot {{appointment.end}}</p>
                                            <p>&nbsp;</p>
                                        </div>
                                        <div class="appointment-element">
                                            <p class="subtitle"><span @click="deleteAppointment(appointment.id)" class="delete-button"><i class="fas fa-trash-alt"></i></span></p>
                                            <p>&nbsp;</p>
                                            <p>&nbsp;</p>
                                            <p>&nbsp;</p>
                                        </div>
                                    </div>
                                </transition>
                            </div>
                        </div>
                    </div>
                    <h2 class="title" v-else>
                        Je hebt geen afspraken.
                    </h2>
                </div>
            </div>
        </section>
    </div>
</template>

<style lang="scss" scoped>
    @import "../styles/appointments.scss";
</style>

<script>
    export default {
        name: 'Appointments',
        data() {
            return {
                appointments: null,
                isAuthenticated: this.$auth.isAuthenticated(), //??
                profile: this.$auth.profile,
                userName: this.$auth.profile.name
            }
        },
        async mounted() {
            await this.getAppointments();
        },
        mounted() {
            this.getAppointments();  
        },
        computed: {
            image() {
                return 
            }
        },
        methods: {
            async getAppointments() {
                try {
                    const accessToken = await this.$auth.getAccessToken();
                    const { data } = await this.$http.get(`${this.$auth.audience}/api/appointments`, {
                        headers: {
                            Authorization: `Bearer ${accessToken}`
                        }
                    });
                    this.appointments = data.value;
                } catch (e) {
                    console.log(e);
                }
            },
            handleLoginEvent(data) {
                this.isAuthenticated = data.loggedIn;
                this.profile = data.profile;
                this.userName = data.profile.name;
            },
            async deleteAppointment(appointmentId) {
                console.log(appointmentId);
                if (!this.appointments.some(element => {
                    return element.id == appointmentId
                })) {
                    console.log('no appt found');
                    return false;
                }
                let appointment = this.appointments.find(element => {
                    return element.id == appointmentId;
                });
                console.log('appt found');
                console.log('try get 204 from server');

                var result = await this.deleteFromStore(appointmentId);

                if (result) {
                    appointment.isCancelled = true;
                    return true;
                }
                return false;
            },
            async deleteFromStore(appointmentId) {
                try {
                    const accessToken = await this.$auth.getAccessToken();
                    const response = await this.$http.delete(`${this.$auth.audience}/api/appointments/delete/${appointmentId}`, {
                        headers: {
                            Authorization: `Bearer ${accessToken}`
                        }
                    });
                    console.log(response);
                    
                    if (response.status == 204) {
                        console.log('server successfully deleted!')
                        return true;
                    }
                } catch(e){
                    console.log(`Something went wrong. Code:${e.response.status}. Text: ${e.response.statusText}`)
                    return null;
                }
            }
        }
    }
</script>