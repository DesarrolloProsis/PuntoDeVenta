<template>
    <div class="home container-fluid">
        <div class="row">
            <div class="col-xl-12">
                <configs></configs>
            </div>
        </div>
    </div>
</template>
<script>import configs from "./components/configs.vue";
import render from "./components/render.vue";

export default {
  name: "home",
  components: {
    configs,
    render
  }
};</script>
