<template>
    <div>
        <div id="parallax-container">
            <div class="layer-one"></div>
            <div class="layer-two"></div>
            <div class="layer-three"></div>
            <div class="layer-four"></div>
            <div class="layer-five"></div>
            <div class="layer-six"></div>
        </div>
        <div id="content">
            <Info>
            </Info>
            <PetOverview>
            </PetOverview>
            <footer class="footer">
                <div class="content has-text-centered">
                    <p>
                        <strong>petshelter</strong> by <a target="_blank" href="https://github.com/CleorgeGooney/petshelter">Mathijs Deprez</a>
                    </p>
                </div>
            </footer>
        </div>
    </div>
</template>

<style lang="scss" scoped>

    .layer-one {
        background: url(../assets/background.svg);
    }

    .layer-two {
        background: url(../assets/clouds1.svg);
    }

    .layer-three {
        background: url(../assets/clouds2.svg);
    }

    .layer-four {
        background: url(../assets/clouds3.svg);
    }

    .layer-five {
        background: url(../assets/moon.svg);
    }

    .layer-six {
        background: url(../assets/hill.svg);
    }

    #parallax-container {
        display: block;
        height: 750px;
    }

        #parallax-container div {
            position: fixed;
            top: 0;
            background-position: center !important;
            transform: translateY(0px);
            height: 1080px;
            width: 100%;
        }

    #content {
        position: relative;
    }
</style>

<script>
    import Info from '../components/Info.vue';
    import PetOverview from '../components/PetOverview.vue';

    export default {
        name: 'Home',
        mounted: function () {
            // this should emit an event vue - wise!!
            window.addEventListener('scroll', () => {
                if (this.$route.name == 'home') { //??
                    let parent = document.getElementById('parallax-container');
                    let children = parent.getElementsByTagName('div');
                    for (let i = 0; i < children.length; i++) {
                        children[i].style.transform = 'translateY(-' + (window.pageYOffset * i / children.length) + 'px)';
                    }
                }
            }, false)
        },
        components: {
            Info,
            PetOverview
        }
    }
</script>
