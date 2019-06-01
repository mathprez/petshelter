<template>
    <div v-if="id">
        <h1 class="title">Update user {{username}}</h1>
        <div class="columns">

            <div class="column">

                <pre>{{current}}</pre>
            </div>
            <div class="column">
                <b-field label="Selecteer een nieuwe shelter">
                    <b-select v-model="selectedShelterId">
                        <option value="">Geen</option>
                        <option v-for="shelter in shelters"
                                :value="shelter.id">
                            {{shelter.name}}
                        </option>
                    </b-select>
                </b-field>
            </div>
        </div>
        <button @click="onSubmit()" class="button is-primary is-fullwidth is-large">Update user</button>
    </div>
</template>

<script>
    export default {
        name: 'UpdateUser',
        data() {
            return {
                shelters: [],
                selectedShelterId: null
            }
        },
        async mounted() {
            await this.getShelters();
        },
        computed: {
            current() {
                return {
                    email: this.email,
                    username: this.username,
                    roles: this.roles,
                    old_shelter_id: this.shelterId,
                    new_shelter_id: this.selectedShelterId,
                    shelter_name: this.shelterName,
                    new_shelter_name: this.selectedShelterName
                }
            },
            selectedShelterName() {

                var vm = this;
                if (!vm.selectedShelterId) {
                    return null;
                }
                var found = vm.shelters.find(x => {
                    return x.id == vm.selectedShelterId
                });
                console.log(found.name);
                return found.name;
            }

        },
        methods: {
            async getShelters() {
                const accessToken = await this.$auth.getAccessToken();
                const { data } = await this.$http.get(`${this.$auth.audience}/api/admin/patch`, {
                    headers: {
                        Authorization: `Bearer ${accessToken}`
                    }
                });

                if (!this.shelters.length) { //?
                    this.shelters = data.shelters;
                }
            },

            async onSubmit() {

                const formData = new FormData();
                formData.append('shelterId', this.selectedShelterId);
                formData.append('userId', this.id);
                console.log(formData);
                const accessToken = await this.$auth.getAccessToken();
                const response = await this.$http.patch(`${this.$auth.audience}/api/admin/patch`, formData, {

                    headers: {
                        Authorization: `Bearer ${accessToken}`
                    }
                });

                console.log(response);

                if (response.status === 200) {
                    this.$toast.open({
                        message: 'Je hebt een shelter toegevoegd aan een user!',
                        type: 'is-success',
                        position: 'is-bottom'
                    });
                    this.$emit('userUpdated', this.id);
                } else {
                    this.$toast.open({
                        message: 'Er is iets foutgelopen aan serverkant!',
                        type: 'is-danger',
                        position: 'is-bottom'
                    })
                }
            }
        },
        props: {
            id: String,
            email: String,
            username: String,
            shelterId: Number,
            shelterName: String,
            roles: Array
        },
        watch: {
            id() {
                this.selectedShelterId = this.shelterId;
            },
            selectedShelterId() {
                if (this.selectedShelterId === "") {
                    this.selectedShelterId = null;
                }
            }
        }
    }
</script>