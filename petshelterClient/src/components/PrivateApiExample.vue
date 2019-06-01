<template>
    <div v-if="isAuthenticated">
        <div>
            <h1 class="title">External API</h1>
            <p>
                Ping an external API by clicking the button below. This will call the external API using an access token, and the API will validate it using
                the API's audience value.
            </p>

            <a class="button is-primary" @click="callApi">Ping</a>
        </div>
        <div v-if="apiMessage">
            <h2>Result</h2>
            <p>{{ apiMessage }}</p>
        </div>
    </div>
</template>

<script>

    export default {
        name: "Api",
        data() {
            return {
                apiMessage: null,
                isAuthenticated: this.$auth.isAuthenticated()
            };
        },
        methods: {
            async callApi() {
                const accessToken = await this.$auth.getAccessToken();

                try {
                    const { data } = await this.$http.get(`${this.$auth.audience}/api/Private`, {
                        headers: {
                            Authorization: `Bearer ${accessToken}`
                        }
                    });

                    this.apiMessage = data.message;
                } catch (e) {
                    this.apiMessage = `Error: the server responded with '${e.response.status}: ${e.response.statusText}'`;
                }
            },
            handleLoginEvent(data) {
                this.isAuthenticated = data.loggedIn;
                this.profile = data.profile;
            }
        }
    };
</script>