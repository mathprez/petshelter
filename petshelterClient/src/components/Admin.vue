<template>
    <section v-if="hasAdminRole" class="hero is-white is-medium ">
        <Users></Users>
    </section>
</template>

<script>
    import Users from '../components/Users.vue';
    export default {
        name: 'Admin',
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
            Users
        },
        computed: {
            // get some design pattern..
            // eg return this.$auth.hasRole('admin')..
            hasAdminRole: {
                get: function () {
                    return this.roles.some(element => {
                        return element === 'admin';
                    });
                }
            }
        }
    };
</script>