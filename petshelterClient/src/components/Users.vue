<template>
    <div class="hero-body">
        <div class="container">
            <div class="columns">
                <div class="column">
                    <h1 class="title">Users</h1>
                    <b-table :data="data"
                             :columns="columns"
                             :selected.sync="selectedUser"
                             focusable>
                    </b-table>
                </div>
            </div>
            <div class="columns">
                <div class="column">
                    <UpdateUser v-bind="selectedUser" @userUpdated="getUsers">
                    </UpdateUser>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import UpdateUser from '../components/UpdateUser.vue';
    export default {
        name: 'Users',
        data() {
            return {
                data: [],
                selectedUser: null,
                columns: [
                    {
                        field: 'id',
                        label: 'ID',
                        width: '40'
                    },
                    {
                        field: 'email',
                        label: 'Email',
                    },
                    {
                        field: 'username',
                        label: 'Username',
                    },
                    {
                        field: 'roles',
                        label: 'Rollen',
                    },
                    {
                        field: 'shelterId',
                        label: 'Shelter ID',
                        numeric: true
                    },
                    {
                        field: 'shelterName',
                        label: 'Sheltername',
                    }
                ]
            }
        },
        async mounted() {
            await this.getUsers();
        },
        methods: {
            async getUsers(userId) {
                try {
                    const accessToken = await this.$auth.getAccessToken();
                    const { data } = await this.$http.get(`${this.$auth.audience}/api/admin/users`, {
                        headers: {
                            Authorization: `Bearer ${accessToken}`
                        }
                    });
                    this.data = data.value.users;
                    if (userId) {
                        this.selectedUser = this.data.find(x => {
                            return x.id == userId;
                        });
                    }
                    else {
                        this.selectedUser = this.data  ? this.data[0] : null;
                    }
                } catch (e) {
                    console.log(e);
                }
            }
        },
        components: {
            UpdateUser
        }
    }
</script>