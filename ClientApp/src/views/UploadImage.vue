<template>
  <b-container fluid>
    <b-card bg-variant="light" class="small-container mb-3">
      <b-form-group label="Use image to:">
        <b-form-radio v-model="selected" value="A">Train model</b-form-radio>
        <b-form-radio v-model="selected" value="B">Recognize face</b-form-radio>
      </b-form-group>
    </b-card>
    <b-card v-if="selected!=''" bg-variant="light" class="small-container mb-3">
      <b-form-group label="Upload image">
        <b-tabs @input="cameraSetup" content-class="mt-3" fill v-model="tabIndex">
          <b-tab title="File">
            <b-form-file placeholder="Upload image..." accept="image/*" v-model="file"></b-form-file>
          </b-tab>

          <b-tab title="Link">
            <b-row>
              <b-col md="1">
                <label>URL:</label>
              </b-col>
              <b-col>
                <b-form-input v-model="imageUrl" placeholder="Paste picture link..."></b-form-input>
              </b-col>
            </b-row>
          </b-tab>

          <b-tab title="Camera">
            <b-button variant="primary" class="mb-2" @click="takePicture">Take Picture</b-button>
            <div>
              <video ref="video" playsinline autoplay></video>
            </div>
          </b-tab>
        </b-tabs>
      </b-form-group>
    </b-card>
    <b-card v-if="isImageUploaded" class="small-container mb-3">
      <b-form-group>
        <h4>Face coordinates:</h4>
        <b-row>
          <b-col>
            <label>x1</label>
            <input v-model="x1" type="number" class="form-control" />
          </b-col>
          <b-col>
            <label>y1</label>
            <input v-model="y1" type="number" class="form-control" />
          </b-col>
          <b-col>
            <label>x2</label>
            <input v-model="x2" type="number" class="form-control" />
          </b-col>
          <b-col>
            <label>y2</label>
            <input v-model="y2" type="number" class="form-control" />
          </b-col>
          <b-col>
            <label>Name</label>
            <input v-model="name" type="string" class="form-control" />
          </b-col>
        </b-row>
      </b-form-group>
      <b-button @click="uploadPicture()">Submit</b-button>
    </b-card>
    <canvas
      ref="imageCanvas"
      v-show="isImageUploaded"
      class="img-thumbnail img-fluid"
      v-draw="link"
    ></canvas>
  </b-container>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import Axios from "axios";

@Component({
  components: {},
  directives: {
    draw: function(canvasElement, binding, vnode) {
      var context = (<HTMLCanvasElement>canvasElement).getContext("2d");

      var img = new Image();
      img.src = binding.value;

      img.onload = () => {
        context!.canvas.width = img.width;
        context!.canvas.height = img.height;
        vnode.context!.$data.isImageUploaded = true;

        var x = vnode.context!.$data.x1;
        var y = vnode.context!.$data.y1;
        var width = vnode.context!.$data.x2 - x;
        var height = vnode.context!.$data.y2 - y;

        context!.strokeStyle = "red";

        context!.drawImage(img, 0, 0);
        context!.beginPath();
        context!.rect(x, y, width, height);
        context!.stroke();
      };

      img.onerror = () => {
        vnode.context!.$data.isImageUploaded = false;
      };
    }
  }
})
export default class UploadImage extends Vue {
  selected: string = "";
  imageUrl: string = "";
  file: File | null = null;
  tabIndex: number = 0;
  cameraLink: string = "";
  cameraReady: boolean = false;
  name: string = "";

  x1: number = 0;
  y1: number = 0;
  x2: number = 0;
  y2: number = 0;

  isImageUploaded: boolean = false;

  destroyed() {
    this.turnOffCamera();
  }

  get fileLink() {
    return this.file ? URL.createObjectURL(this.file) : "";
  }

  get link() {
    switch (this.tabIndex) {
      case 0:
        return this.fileLink;
        break;
      case 1:
        return this.imageUrl;
        break;
      case 2:
        return this.cameraLink;
        break;
    }
  }

  handleSuccess(stream: MediaStream) {
    this.video.srcObject = stream;
    this.cameraReady = true;
  }

  handleError(error: Error) {
    console.log(
      "navigator.MediaDevices.getUserMedia error: ",
      error.message,
      error.name
    );
  }

  cameraSetup() {
    if (this.tabIndex != 2) {
      this.turnOffCamera();

      return;
    }

    navigator.mediaDevices
      .getUserMedia({ audio: false, video: true })
      .then(this.handleSuccess)
      .catch(this.handleError);
  }

  turnOffCamera() {
    (<MediaStream>this.video.srcObject!).getTracks().forEach((track: any) => {
      track.stop();
    });
  }

  get video() {
    return <HTMLVideoElement>this.$refs["video"];
  }

  takePicture() {
    var canvas = document.createElement("canvas");
    canvas.width = this.video.videoWidth;
    canvas.height = this.video.videoHeight;
    var canvasContext = canvas.getContext("2d");
    canvasContext!.drawImage(this.video, 0, 0);

    this.cameraLink = canvas.toDataURL("image/png");
  }

  uploadPicture() {
    var base64 = (<any>this.$refs["imageCanvas"]).toDataURL("image/png");
    Axios.post("/api/Image", {
      ImageBlob: base64.replace(/^data:image\/(png|jpg);base64,/, ""),
      ImageLink: this.tabIndex == 1 ? this.imageUrl : "",
      TaggedFaces: [
        {
          X1: Number(this.x1),
          Y1: Number(this.y1),
          Y2: Number(this.y2),
          X2: Number(this.x2),
          Name: this.name
        }
      ]
    })
      .then(() => {
        this.tabIndex = 0;
        this.selected = "";
        this.imageUrl = "";
        this.turnOffCamera();
        this.cameraLink = "";
      })
      .catch((error: any) => {
        console.log(error);
      });
  }
}
</script>

<style scoped>
.small-container {
  margin: auto;
  width: 50%;
}

input[type="number"]::-webkit-inner-spin-button,
input[type="number"]::-webkit-outer-spin-button {
  -webkit-appearance: none;
  -moz-appearance: none;
  appearance: none;
  margin: 0;
}
</style>