<template>
  <b-container fluid>
    <b-pagination-nav :link-gen="linkGen" :number-of-pages="pageCount" use-router align="center"></b-pagination-nav>
    <div v-if="taggedImages.length">
      <b-row>
        <b-col lg="3" md="4" xs="6" fluid v-for="(image,i) in taggedImages" :key="i">
          <div>
            <canvas
              @click="modalOpen(image)"
              style="cursor: pointer;"
              class="img-thumbnail img-fluid"
              v-draw="image"
            ></canvas>
          </div>
        </b-col>
      </b-row>
    </div>
    <div v-else>
      <h3>LOADING...</h3>
    </div>
    <b-modal id="myModal" :title="tempName">
      <b-img fluid-grow :src="tempBlob"></b-img>
      <template v-slot:modal-footer="{cancel}">
        <b-button size="sm" variant="info" @click="cancel()">Cancel</b-button>
        <b-button size="sm" variant="danger" @click="deleteImage()">DELETE</b-button>
      </template>
    </b-modal>
    <b-pagination-nav :link-gen="linkGen" :number-of-pages="pageCount" use-router align="center"></b-pagination-nav>
  </b-container>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { TaggedImage } from "@/models/TaggedImage";
import { BvModal } from "bootstrap-vue";
import axios from "axios";
import { Prop, Watch } from "vue-property-decorator";

@Component({
  directives: {
    draw: function(canvasElement, binding) {
      var context = (<HTMLCanvasElement>canvasElement).getContext("2d");
      var taggedImage = <TaggedImage>binding.value;

      var img = new Image();
      img.src = "data:image/png;base64," + taggedImage.imageBlob;

      img.onload = () => {
        context!.canvas.width = img.width;
        context!.canvas.height = img.height;

        context!.drawImage(img, 0, 0);

        context!.strokeStyle = "red";

        context!.beginPath();
        taggedImage.taggedFaces.forEach(element => {
          context!.rect(
            element.x1,
            element.y1,
            element.x2 - element.x1,
            element.y2 - element.y1
          );
        });
        context!.stroke();
      };
    }
  }
})
export default class Gallery extends Vue {
  taggedImages: TaggedImage[] = [];
  pageCount: number = 0;
  currentPage: number = 1;

  refreshImages: boolean = false;

  mounted() {
    this.getImages(Number(this.$route.params["pageNum"]));
  }

  @Watch("$route", { deep: true })
  onChange(newPageNum: any) {
    this.getImages(Number(newPageNum.params.pageNum));
  }

  tempBlob: string = "";
  tempId: number = 0;
  tempName: string = "";

  private getImages(pageNum: number) {
    this.taggedImages = [];
    axios
      .get("/api/Image/getByPageNum?pageNum=" + pageNum)
      .then(response => {
        this.taggedImages = response.data.image;
        this.pageCount = response.data.pageCount;
        this.currentPage = pageNum;
      })
      .catch(function(error) {
        // handle error
        console.log(error);
      });
  }

  modalOpen(image: TaggedImage): void {
    this.tempBlob = "data:image/png;base64," + image.imageBlob;
    this.tempId = image.id;
    this.tempName = image.taggedFaces[0].name;

    this.$bvModal.show("myModal");
  }

  linkGen(pageNum: number) {
    return { path: `/gallery/${pageNum}` };
  }

  deleteImage(): void {
    axios
      .delete(`/api/image/${this.tempId}`)
      .then(() => {
        this.$bvModal.hide("myModal");
        this.getImages(this.currentPage);
      })
      .catch((error: Error) => {
        console.log(error);
      });
  }
}
</script>

<style scoped>
</style>