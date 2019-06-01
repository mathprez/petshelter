<template>
    <div>
        <nav class="navbar is-fixed-top" role="navigation" aria-label="main navigation">
            <div class="navbar-brand">

                <router-link class="navbar-item" to="/">
                    <img src="../assets/banner.svg">
                </router-link>

                <a v-if="isAuthenticated"
                   class="navbar-burger burger is-center min-1088"
                   role="button"
                   aria-label="profile"
                   aria-expanded="false"
                   data-target="petshelterNavbar"
                   @click.prevent="toggleProfile">

                    <i aria-hidden="true" class="fas fa-user fa-lg"></i>
                </a>

                <a @click.prevent="showBurgerMenu = !showBurgerMenu"
                   role="button"
                   aria-label="menu"
                   aria-expanded="false"
                   class="navbar-burger burger"
                   v-bind:class="{'remove-margin-left': isAuthenticated}"
                   data-target="petshelterNavbar">

                    <span aria-hidden="true"></span>
                    <span aria-hidden="true"></span>
                    <span aria-hidden="true"></span>
                </a>
            </div>
            <div id="petshelterNavbar" class="navbar-menu" v-bind:class="{'is-active': showBurgerMenu}">
                <div class="navbar-start">
                    <router-link to="/" class="navbar-item">Home</router-link>
                    <router-link to="/" v-scroll-to="'#searchHeader'" class="navbar-item">Huisdieren</router-link>
                    <router-link to="/Appointments" v-if="isAuthenticated" class="navbar-item">Mijn afspraken</router-link>

                    <div v-if="isAuthenticated && hasVolunteerRole" class="navbar-item has-dropdown is-hoverable">
                        <a class="navbar-link">Vrijwilligers</a>
                        <div class="navbar-dropdown">
                            <router-link to="/Volunteers" class="navbar-item">Nieuw huisdier</router-link>
                        </div>
                    </div>

                    <router-link to="/Admin" v-if="isAuthenticated && hasAdminRole" class="navbar-item">Admin</router-link>
                   
                    <div class="navbar-item has-dropdown is-hoverable">
                        <a class="navbar-link">Meer</a>
                        <div class="navbar-dropdown">
                            <a class="navbar-item">Over ons</a>
                            <a class="navbar-item">Contact</a>
                        </div>
                    </div>
                </div>
                <div class="navbar-end">
                    <a v-if="isAuthenticated"
                       style="display: none;"
                       class="is-center is-square fake-burger-profile"
                       role="button"
                       aria-label="profile"
                       @click.prevent="showProfile = !showProfile">
                        <i aria-hidden="true" class="fas fa-user fa-lg"></i>
                    </a>
                    <div class="navbar-item">
                        <div class="buttons">
                            <a v-if="!isAuthenticated" href="#" @click.prevent="login" class="button is-primary">
                                <strong>Log in</strong>
                            </a>
                            <a v-if="isAuthenticated"
                               href="#"
                               @click.prevent="logout"
                               class="button is-inverse">Logout</a>
                        </div>
                    </div>
                </div>
            </div>
        </nav>
        <transition name="fade">
            <Profile v-if="showProfile"></Profile>
        </transition>
    </div>
</template>

<script>
    import Profile from "../components/Profile.vue";

    export default {
        name: "Navbar",
        data() {
            return {
                isAuthenticated: false,
                profile: null,
                roles: [],

                //this.$auth.profile !== null
                //    ? this.$auth.profile[this.$auth.roleIdentifier] //not correct?
                //    :

                showProfile: false,
                showBurgerMenu: false
            };
        },
        methods: {
            login() {
                this.$auth.login();
            },
            logout() {
                this.$auth.logOut();
            },
            handleLoginEvent(data) {
                this.isAuthenticated = data.loggedIn;
                this.profile = data.profile;
                if (data.profile) {
                    this.roles = data.profile[this.$auth.roleIdentifier] || [];
                }
            },
            toggleProfile() {
                if (this.showBurgerMenu) {
                    this.showBurgerMenu = false;
                    this.showProfile = true;
                } else {
                    this.showProfile = !this.showProfile;
                }
            }
        },
        computed: {
            hasVolunteerRole: {
                get: function () {
                    return this.roles.some(element => {
                        return element === 'volunteer';
                    });
                }
            },
            hasAdminRole: {
                get: function () {
                    return this.roles.some(element => {
                        return element === 'admin';
                    });
                }
            }
        },
        components: {
            Profile
        }
    };
</script>

<style lang="scss" scoped>

    .fake-burger-profile {
        color: #4a4a4a;
        cursor: pointer;
        height: 50px;
        width: 50px;
    }

        .fake-burger-profile:hover {
            background-color: rgba(0, 0, 0, 0.05);
        }

    @media screen and (min-width: 1088px) {
        .fake-burger-profile {
            display: inline-flex !important;
        }

        .navbar-burger {
            display: none !important;
        }
    }


    .remove-margin-left {
        margin-left: 0;
    }

    .is-center {
        display: flex;
        justify-content: center;
        align-items: center;
    }
</style>