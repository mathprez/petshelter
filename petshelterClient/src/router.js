import Vue from 'vue';
import Router from 'vue-router';
import auth from "./services/authservice";

import Callback from './components/Callback.vue';
import Home from './components/Home.vue';
import Volunteers from './components/Volunteers.vue'
import CreateAppointment from './components/CreateAppointment.vue';
import Appointments from './components/Appointments.vue'
import Admin from './components/Admin.vue'

Vue.use(Router);

const routes = [
    {
        path: '/',
        name: 'home',
        component: Home
    },
    {
        path: '/volunteers',
        name: 'volunteers',
        component: Volunteers
    },
    {
        path: '/callback',
        name: 'callback',
        component: Callback
    },
    {
        path: '/createappointment/:petId',
        name: 'createappointment',
        component: CreateAppointment
    },
    {
        path: '/appointments',
        name: 'appointments',
        component: Appointments
    },
    {
        path: '/admin',
        name: 'admin',
        component: Admin
    }
];

const router = new Router({
    mode: 'history',
    base: process.env.BASE_URL,
    routes
});

router.beforeEach((to, from, next) => {
    //maybe create an array with public views
    if (to.path === "/" || to.path === "/callback" || auth.isAuthenticated()) {
        return next();
    }

    // Specify the current path as the customState parameter, meaning it
    // will be returned to the application after auth
    auth.login({ target: to.path });
});


export default router;