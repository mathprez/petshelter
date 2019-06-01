<template>
    <div>
        <p>Loading...</p>
    </div>
</template>

<script>
    export default {
        name: 'Callback',
  methods: {
    handleLoginEvent(data) {
      this.$router.push(data.state.target || "/");
    }
  },
  created() {
    this.$auth.handleAuthentication();
  }
};
</script>