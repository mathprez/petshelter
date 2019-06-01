<template>
    <div>
        <Navbar></Navbar>
        <div class="has-navbar-fixed-top">
            <router-view></router-view>
        </div>
    </div>
</template>

<script>
    import Navbar from './components/Navbar.vue';
    export default {
        name: "app",
        async created() {
            try {
                await this.$auth.renewTokens();
            } catch (e) {
                window.console.log(e);
            }
        },
        components: {
            Navbar
        }
    };
</script>
<style lang="scss">
    @import "styles/petshelterstyles";
</style>