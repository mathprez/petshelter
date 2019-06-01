import Vue from 'vue';
import App from './App.vue';
import AuthPlugin from "./plugins/auth.js";
import router from './router';
import axios from 'axios';
import VueAxios from 'vue-axios';
import Buefy from 'buefy';
import VueScrollTo from 'vue-scrollto';
import VeeValidate from 'vee-validate';

Vue.use(VeeValidate, {
});
Vue.use(AuthPlugin);
Vue.use(Buefy);
Vue.use(VueAxios, axios);
Vue.use(VueScrollTo, {
    container: "body",
    duration: 500,
    easing: "ease",
    offset: 0,
    force: true,
    cancelable: true,
    onStart: false,
    onDone: false,
    onCancel: false,
    x: false,
    y: true
});

Vue.config.productionTip = false;

new Vue({
    router,
    render: h => h(App),
}).$mount('#app');