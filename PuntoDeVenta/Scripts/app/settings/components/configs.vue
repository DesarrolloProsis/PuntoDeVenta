<template>
    <div>
        <div class="row">
            <div class="col-xl-3">
                <div class="space">
                    <b-list-group class="shadow">
                        <b-list-group-item button class="title-list" disabled>Configuración</b-list-group-item>
                        <b-list-group-item @click="seleccion = '1'" button>Cantidades</b-list-group-item>
                        <!--DESCOMENTAR PARA AGREGAR CONGIFURACIONES-->
                        <!--
                        <b-list-group-item @click="seleccion = '2'" button>Administrar 2</b-list-group-item>
                        <b-list-group-item @click="seleccion = '3'" button>Administrar 3</b-list-group-item>-->
                    </b-list-group>
                </div>
            </div>
            <div class="col-xl-9">
                <render :seleccion="seleccion"></render>
            </div>
        </div>
    </div>
</template>

<script>
import render from "./render.vue";
export default {
  data() {
    return {
      seleccion: '1'
    };
  },
  components: {
    render
  }
};
</script>
<style>
    .title-list {
        background-color: #00a73c !important;
        color: white !important;
    }

    .space {
        margin-bottom: 20px;
    }
</style>