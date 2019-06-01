<template>
    <div>
        <div class="card card-padding">
            <div class="card-image">
                <figure>
                    <img :src="image"
                         alt="">
                </figure>
            </div>
            <div class="card-content">
                <div class="media">
                    <div class="media-left">
                        <figure v-if="gender === 1" class="image is-48x48">
                            <i class="fas fa-mars fa-3x" alt="Placeholder image"></i>
                        </figure>

                        <figure v-else="" class="image is-48x48">
                            <i class="fas fa-venus fa-3x" alt="Placeholder image"></i>
                        </figure>
                    </div>
                    <div class="media-content">
                        <p class="title is-4"> {{ name }}</p>
                        <p class="subtitle is-6">@{{ breed }}</p>
                    </div>
                    <div class="media-right">
                        <b-tag>{{shelter}}</b-tag>
                    </div>
                </div>
                <div class="content">
                    {{description}}
                </div>
                
                <b-button tag="router-link" :to="'/createappointment/' + id" type="is-primary" outlined>Maak een afspraak!</b-button>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'PetCard',
        data() {
            return {
                pet: null
            };
        },
        computed: {
            image() {
                return this.externalImage || this.base64Image;
            }
        },
        props: {
            id: Number,
            name: String,
            externalImage: String,
            base64Image: String,
            gender: Number,
            breed: String,
            description: String,
            shelter: String
        }
    }
</script>

<style scoped>
    .card-padding {
        margin-top: 40px;
        margin-bottom: 40px;
    }
</style>