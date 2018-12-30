<template>
  <v-app dark>
    <v-content app>
      <v-layout row wrap>
        <v-flex xs12 text-xs-center pa-0 ma-0>
          <v-system-bar window>
            <v-toolbar-items>
              <v-menu offset-y>
                <v-btn class="text" flat slot="activator">File</v-btn>
                <v-list dense>
                  <v-list-tile>
                    <v-list-tile-title v-on:click="viewModel.State = 'open'">Open</v-list-tile-title>
                  </v-list-tile>
                </v-list>
              </v-menu>
            </v-toolbar-items>
          </v-system-bar>
        </v-flex>
        <v-flex xs4 style="height: 500px; overflow-y: scroll">
          <v-treeview :items="viewModel.TreeNodes"
            :active.sync="activeNodes"
            :open.sync="openNodes"
            item-key="Name"
            item-text="Name"
            item-children="Children"
            activatable
            hoverable
            ref="treeView"
            v-on:click="selectNode"
            v-on:update:active="updateActive">
            <template slot="prepend" slot-scope="{ active, indeterminate, item, leaf, open, selected }">
              <v-icon v-if="!item.IsMilo" small color="primary">{{open ? 'fa-folder-open' : 'fa-folder'}}</v-icon>
              <v-icon v-else small color="primary">{{fileIcons[item.Type] || 'fa-file'}}</v-icon>
            </template>
            <template slot="append" slot-scope="{ active, indeterminate, item, leaf, open, selected }">

            </template>
          </v-treeview>
        </v-flex>
        <!--<v-flex xs8>
          <strong v-if="activeNode">{{activeNode.Name}} ({{activeNode.Type}})</strong>
        </v-flex>-->
        <v-flex xs8 ref="mainScene" v-on:click="getTexture">
          <canvas id="renderer" style="width: 100%; height: 100%; background-color: black"></canvas>
        </v-flex>
      </v-layout>
    </v-content>
  </v-app>
  <!--<div class="app">
    <img src="./assets/logo.png">
    <h1>{{Message}}</h1>
    <h2>Related Links</h2>
    <ul>
      <li><a target="_blank" href="https://vuejs.org">Core Vue Docs</a></li>
      <li><a target="_blank" href="https://forum.vuejs.org">Vue Forum</a></li>
      <li><a target="_blank" href="https://github.com/NeutroniumCore/Neutronium">Neutronium Project</a></li>
      <li><a target="_blank" href="https://github.com/NeutroniumCore/neutronium-vue">Neutronium Vue client</a></li>
      <li><a target="_blank" href="https://github.com/NeutroniumCore/Neutronium.Template/blob/master/README.md">Template documentation</a></li>
    </ul>
  </div>-->
</template>

<script>
  import { toPromise } from 'neutronium-vue-resultcommand-topromise'

  // ThreeJS
  // TODO: Extract out to seperate component
  import * as THREE from 'three'

  const props={
    viewModel: Object,
    __window__: Object
  };

  export default {
    name: 'app',
    props,
    data: function () {
      return {
        fileIcons: {
          Tex: 'fa-file-image',
          View: 'fa-shapes',
          Group: 'fa-shapes'
        },
        activeNodes: [],
        openNodes: [],
        activeNode: null
      }
    },
    computed: {
    },
    methods: {
      getTexture: function (texNode) {
        console.log(texNode)

        toPromise(this.viewModel.GetTexture, texNode)
          .then((result) => {
            this.createScene(result)

            console.log(result)
            console.log(typeof result)
          }, (error) => {
            console.log('error')
          })
        
        //this.viewModel.GetTexture.Execute(node, result => console.log('what'))
      },
      selectNode: function (ev) {
        console.log(ev)
      },
      updateActive: function (activeNodes) {
        if (activeNodes.length <= 0) {
          this.activeNode = null
          return // TODO: Update displayed data?
        }

        if (this.viewModel.GroupByType) {

          for (let node of this.viewModel.TreeNodes) {
            for (let child of node.Children) {
              if (child.Name == activeNodes[0]) {
                this.activeNode = child
                return
              }
            }
          }

          /*
          this.viewModel.TreeNodes.forEach((node, idx) => {

            node.Children.forEach((child, jdx) => {
              let isActive = child.Name == activeNodes[0]

              if (isActive) {

                this.activeNode = child
                return
              }
            })
          })*/

          this.activeNode = null
        }

        this.activeNode = null
      },
      createScene: function (tex) {
        let scene = new THREE.Scene()
        window.scene = scene // Allows ThreeJS extension to find view
        
        let camera = new THREE.PerspectiveCamera(70, 1, 0.1, 1000)

        let renderElm = document.getElementById('renderer')

        let renderer = new THREE.WebGLRenderer({ canvas: renderElm })
        renderer.domElement.id = 'renderer'

        //renderer.setSize(500, 300)
        
        let geometry = new THREE.BoxGeometry(1, 1, 1)
        let material = new THREE.MeshBasicMaterial({ color: 0x00ff00 })
        let cube = new THREE.Mesh(geometry, material)

        // From node
        let texture = new THREE.DataTexture(new Uint8Array(tex.Data), tex.Width, tex.Height, THREE.RGBAFormat)
        texture.needsUpdate = true
        //scene.add(texture)

        let spriteMat = new THREE.SpriteMaterial({ map: texture })
        let sprite = new THREE.Sprite(spriteMat)
        scene.add(sprite)

        //scene.add(cube)
        camera.position.z = 5
        
        function animate() {
	        requestAnimationFrame(animate)
          renderer.render(scene, camera)
        
          cube.rotation.x += 0.01;
          cube.rotation.y += 0.01;
        }
        
        animate()
        
        let textureLoader = new THREE.TextureLoader()
        //let tex = textureLoader.load("https://www.transitionculture.org/wp-content/uploads/guitar-hero-1.jpg")
        //console.log(tex)
        
        //THREE.DefaultLoadingManager.
        
        //textureLoader.load("C:/Users/Cisco/Pictures/gamecube.jpg")
        
        //let tex = new Uint8Array(256 * 3)
        //let data = new THREE.DataTexture(tex, 8, 8, THREE.RGBFormat)
        
        //console.log(tex)
        //console.log(data)
        
        this.$refs.mainScene.appendChild(renderer.domElement)
      }
    },
    watch: {
      activeNode: function (node) {
        if (!node || !node.IsMilo) {
          return
        }

        if (node.Type !== 'Tex') {
          return
        }

        this.getTexture(node)
      }
    },
    mounted: function () {
      //this.createScene()
    }
  }
</script>

<style scoped>
  .text {
    text-transform: none;
  }

  #mainScene > canvas {
    width: 100%;
    height: 100%;
  }

</style>

<style>
  /* width */
  ::-webkit-scrollbar {
      width: 10px;
  }
  
  /* Track */
  ::-webkit-scrollbar-track {
      background: #f1f1f1; 
  }
   
  /* Handle */
  ::-webkit-scrollbar-thumb {
      background: #888; 
  }
  
  /* Handle on hover */
  ::-webkit-scrollbar-thumb:hover {
      background: #555; 
  }
</style>
