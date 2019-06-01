<template>
    <section class="hero is-white is-small">
        <div class="hero-body">
            <div class="container">
                <h1 class="title">Zoek een huisdier</h1>
                <div class="searcharea" grouped>
                    <div class="columns">
                        <div class="column">
                            <b-field label="Categorie">
                                <b-select v-model="selectedCategory" placeholder="Alles" expanded>
                                    <option value="-1" selected>Alles</option>
                                    <option v-for="category in categories" :value="category.id">{{category.name}} </option>
                                </b-select>
                            </b-field>
                        </div>
                        <div class="column">
                            <b-field label="Soort">
                                <b-select v-model="selectedBreed" placeholder="Alles" expanded>
                                    <option value="-1" selected>Alles</option>
                                    <option v-for="breed in breeds" :value="breed.id">{{breed.name}} </option>
                                </b-select>
                            </b-field>
                        </div>
                        <div class="column">
                            <b-field label="Geslacht">
                                <b-select v-model="selectedGender" placeholder="Alles" expanded>
                                    <option value="-1" selected>Alles</option>
                                    <option value="1">Mannetje</option>
                                    <option value="2">Vrouwtje</option>
                                </b-select>
                            </b-field>
                        </div>
                        <div class="column">
                            <b-field label="Shelter">
                                <b-select v-model="selectedShelter" placeholder="Alles" expanded>
                                    <option value="-1" selected>Alles</option>
                                    <option v-for="shelter in shelters" :value="shelter.id">{{shelter.name}} </option>
                                </b-select>
                            </b-field>
                        </div>
                    </div>
                    <div class="columns">
                        <div class="column">
                            <b-button id="searchHeader" type="is-primary" class="is-fullwidth" size="is-large" v-on:click.prevent="function(){if (pageNumber === 1){callApi()}else{pageNumber=1}}">
                                Zoek
                            </b-button>
                        </div>
                    </div>
                </div>
                <div class="columns">
                    <div class="column" v-for="pets in groupedPets(3)">
                        <PetCard v-for="pet in pets" v-bind="pet"></PetCard>
                    </div>
                </div>

                <b-pagination v-scroll-to="'#searchHeader'"
                              :total="count"
                              :current.sync="pageNumber"
                              :simple="false"
                              icon-pack="fas"
                              :per-page="pageSize"
                              aria-next-label="Next page"
                              aria-previous-label="Previous page"
                              aria-page-label="Page"
                              aria-current-label="Current page">
                </b-pagination>

            </div>
        </div>
    </section>
</template>

<script>
    import PetCard from '../components/PetCard.vue';
    export default {
        name: 'PetOverview',
        data() {
            return {
                pets: [],
                categories: [],
                shelters: [],
                selectedShelter: -1,
                selectedCategory: -1,
                selectedBreed: -1,
                selectedGender: -1,
                pageNumber: 1,
                pageSize: 12,
                count: 0
            };
        },
        components: {
            PetCard
        },
        methods: {
            groupedPets(columns) {
                if (this.pets === null || this.pets === undefined || !this.pets.length) {
                    window.console.log("no pets");
                } else {
                    window.console.log("getting pets in chunks");
                    var count = this.pets.length;
                    var chunk = Math.floor(count / columns);

                    if (chunk === 0) {
                        return [this.pets]
                    }

                    var grouped = [];
                    for (var i = 0; i < count; i += chunk) {
                        if (i + chunk > count) {
                            grouped[0].push(this.pets.slice(i, i + chunk))
                        } else {
                            grouped.push(this.pets.slice(i, i + chunk));
                        }
                    }

                    return grouped;
                }
            },
            async callApi() {
                window.console.log('calling api');
                try {
                    const { data } = await this.$http.get(`${this.$auth.audience}/api/pets`, {
                        params: {
                            pageNumber: this.pageNumber,
                            pageSize: this.pageSize,
                            breedId: this.selectedBreed,
                            genderId: this.selectedGender,
                            categoryId: this.selectedCategory,
                            shelterId: this.selectedShelter
                        }
                    });

                    this.count = data.payload.count;
                    this.pets = data.payload.pets;
                } catch (e) {
                    this.pets = `Error: the server responded with '${e.response.status}: ${e.response.statusText}'`;
                }
            }
        },
        computed: {
            breeds: {
                get: function () {
                    var vm = this;
                    if (this.selectedCategory !== undefined && this.selectedCategory !== -1 && this.selectedCategory !== null) {

                        return this.categories.find(function (value, index) {
                            return value.id == vm.selectedCategory;
                        }).breeds;
                    }
                    return [];
                }
            }
        },
        mounted() {
            this.$http.get(`${this.$auth.audience}/api/pets/searchData`)
                .then(response => {
                    this.categories = response.data.categories;
                    this.shelters = response.data.shelters;
                });
        },
        handleLoginEvent(data) {
            this.isAuthenticated = data.loggedIn;
            this.profile = data.profile;
        },
        watch: {
            //this feels wrong
            pageNumber() {
                this.callApi();
            }
        }
    }
</script>