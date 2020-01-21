<template>
  <div id="app">
    <b-navbar fixed="top" type="dark" variant="dark">
      <b-navbar-brand to="/">ImeProjekta</b-navbar-brand>
      <b-navbar-nav>
        <b-nav-item to="/gallery/1">Gallery</b-nav-item>
        <b-nav-item to="/uploadImage">Upload Image</b-nav-item>
      </b-navbar-nav>

      <b-navbar-nav class="ml-auto">
        <b-nav-item to="/about">About</b-nav-item>
      </b-navbar-nav>
    </b-navbar>
    <router-view></router-view>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

@Component({

})
export default class App extends Vue {

}
</script>

<style lang="less">
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
