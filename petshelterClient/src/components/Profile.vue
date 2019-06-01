<template>
    <div class="profile card" v-if="profile">
        <div class="card-content">
            <div class="media">
                <div class="media-left">
                    <figure class="image is-48-48">
                        <img :src="profile.picture" alt="thumb userimage">
                    </figure>
                </div>
                <div class="media-content">
                    <p class="title is-4">{{profile.name}}</p>
                    <p class="subtitle is-6">{{profile.email}}</p>
                    <p class="subtitle is-6">@{{profile.nickname}}</p>
                </div>
            </div>

            <div class="content">
                <p v-if="roles" class="title is-6"> {{roles}} </p>
                <a>@favos</a>
                <br>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                profile: this.$auth.profile,
                roles:
                    this.$auth.profile !== null
                        ? this.$auth.profile[this.$auth.roleIdentifier]
                        : []
            };
        },
        methods: {
            handleLoginEvent(data) {
                this.profile = data.profile;
                if (data.profile) {
                    this.roles = data.profile[this.$auth.roleIdentifier] || [];
                }
            }
        }
    };
</script>

<style scoped>
    .profile {
        right: 0;
        position:fixed;
        background-color: white;
        z-index: 1;
        margin-top: 52px;
    }

    .fade-enter-active,
    .fade-leave-active {
        transition: opacity 0.5s;
    }

    .fade-enter,
    .fade-leave-to {
        opacity: 0;
    }
</style>