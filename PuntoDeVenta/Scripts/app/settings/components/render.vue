<template>
    <div class="shadow">
        <b-card title="Card title" sub-title="Card subtitle">
            <div class="container">
                <div v-if="seleccion === '1'">
                    <ajusteuno></ajusteuno>
                </div>
                <!--Descomentar para agregar funciones
                <div v-else-if="seleccion === '2'">
                    <ajustedos></ajustedos>
                </div>
                <div v-else-if="seleccion === '3'">
                    <ajustetres></ajustetres>
                </div>-->
            </div>
        </b-card>
    </div>
</template>
<script>
    //Importar y agregar componentes depenediendo de las funciones
    import ajusteuno from "./configs/ajusteuno.vue";

export default {
  name: "render",
  props: ["seleccion"],
  components: {
      ajusteuno,
  }
};
</script>