<template>
    <div>
        <h1 class="title">Nieuw huisdier</h1>
        <form @submit.prevent="onSubmit">
            <b-field :type="{'is-danger': errors.has('name')}"
                     :message="errors.first('name')"
                     label="Name">
                <b-input v-model="name" name="name" v-validate="'required|min:2|max:50'"></b-input>
            </b-field>

            <b-field grouped>
                <b-field label="Categorie"
                         :type="{'is-danger': errors.has('category')}"
                         :message="errors.first('category')">
                    <b-select v-model="category" name="category" placeHolder="Selecteer een categorie" v-validate="'required'">
                        <option v-for="category in categories" :value="category.id">
                            {{category.name}}
                        </option>
                    </b-select>
                </b-field>

                <b-field label="Type"
                         :type="{'is-danger': errors.has('breed')}"
                         :message="errors.first('breed')"
                         expanded>
                    <b-select v-model="breed" name="breed" placeHolder="Selecteer een soort" v-validate="'required'">
                        <option v-for="breedd in breeds" :value="breedd.id">
                            {{breedd.name}}
                        </option>
                    </b-select>
                </b-field>
            </b-field>

            <b-field label="Beschrijving"
                     v-bind:type="{'is-danger': errors.has('description')}"
                     :message="errors.first('description')">
                <b-input type="textarea" v-model="description" name="description" placeHolder="Geef een korte beschrijving" v-validate="'required|min:10|max:700'"></b-input>
            </b-field>

            <b-field grouped>

                <b-field label="Kleur"
                         :type="{'is-danger': errors.has('color')}"
                         :message="errors.first('color')">
                    <b-select v-model="color" name="color" placeHolder="Selecteer een kleur" v-validate="'required'">
                        <option v-for="color in colors" :value="color">
                            {{color}}
                        </option>
                    </b-select>
                </b-field>

                <b-field label="Mannetje of vrouwtje"
                         :type="{'is-danger': errors.has('gender')}"
                         :message="errors.first('gender')"
                         expanded>
                    <b-select v-model="gender" name="gender" placeHolder="Mannetje of vrouwtje" v-validate="'required'">
                        <option v-for="gender in genders" :value="gender.id">
                            {{gender.name}}
                        </option>
                    </b-select>
                </b-field>
            </b-field>

            <b-field class="file"
                     :type="{'is-danger': errors.has('file')}"
                     :message="errors.first('file')">
                <b-upload v-model="file" name="file" v-validate="'required'">
                    <a class="button is-primary">
                        <b-icon pack="fas" icon="upload"></b-icon>
                        <span>Klik om een foto te selecteren</span>
                    </a>
                </b-upload>
                <span class="file-name" v-if="file">
                    {{ file.name }}
                </span>
            </b-field>
            <button  style="margin-top:40px" class="button is-primary is-fullwidth is-large" type="submit">Upload huisdier</button>
        </form>
    </div>
</template>

<script>
    export default {
        name: 'CreatePet',
        data() {
            return {
                name: null,
                description: null,
                categories: null,
                category: null,
                breed: null,
                file: null,
                colors: null,
                color: null,
                genders: null,
                gender: null
            }
        },
        async mounted() {
            const accessToken = await this.$auth.getAccessToken();
            const { data } = await this.$http.get(`${this.$auth.audience}/api/pets/create`, {

                headers: {
                    Authorization: `Bearer ${accessToken}`
                }

            });

            this.categories = data.value.categories;
            this.colors = data.value.colors;
            this.genders = data.value.genders;
        },
        methods: {

            async validateAll() {
                var result = await this.$validator.validateAll();

                if (result) {

                }
                return result;
            },
            async onSubmit() {
                var validationResult = await this.validateAll();
                if (!validationResult) {
                    this.$toast.open({
                        message: 'Niet alles is ingevuld.',
                        type: 'is-danger',
                        position: 'is-bottom'
                    });
                    return;
                }
                const formData = new FormData();
                formData.append('image', this.file, this.file.name);
                formData.append('name', this.name);
                formData.append('breedId', this.breed);
                formData.append('gender', this.gender);
                formData.append('description', this.description);
                formData.append('color', this.color)
                console.log(formData);
                const accessToken = await this.$auth.getAccessToken();
                const response = await this.$http.post(`${this.$auth.audience}/api/pets/create`, formData, {

                    headers: {
                        Authorization: `Bearer ${accessToken}`
                    }
                });

                console.log(response);

                if (response.status === 201) {
                    this.$toast.open({
                        message: 'Je hebt een nieuw huisdier toegevoegd!',
                        type: 'is-success',
                        position: 'is-bottom'
                    })
                } else {
                    this.$toast.open({
                        message: 'Er is iets foutgelopen aan serverkant!',
                        type: 'is-danger',
                        position: 'is-bottom'
                    })
                }
            }
        },
        computed: {
            breeds: {
                get: function () {

                    var vm = this;
                    if (this.category) {

                        return this.categories.find(function (value, index) {
                            return value.id == vm.category;
                        }).breeds;
                    }
                    return [];
                }
            }
        }
    }
</script>