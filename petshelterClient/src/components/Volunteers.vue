<template>
        <section v-if="hasVolunteerRole" class="hero is-white is-medium ">
            <div class="hero-body">
                <div class="container">
                    <CreatePet></CreatePet>
                </div>
            </div>
        </section>
</template>

<script>
    import CreatePet from '../components/CreatePet.vue';
    export default {
        name: 'Volunteers',
        data() {
            return {
                profile: this.$auth.profile,
                roles: this.$auth.profile[this.$auth.roleIdentifier]
            };
        },
        methods: {
            handleLoginEvent(data) {
                this.profile = data.profile;
                this.roles = data.profile[this.$auth.roleIdentifier];
            }
        },
        components: {
            CreatePet
        },
        computed: {
            // get some code pattern..
            // retun this.$auth...
            hasVolunteerRole: {
                get: function () {
                    return this.roles.some(element => {
                        return element === 'volunteer';
                    });
                }
            }
        }
    };
</script>