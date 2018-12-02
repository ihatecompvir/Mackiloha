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
        <v-flex xs4 style="height: 500px; overflow-y: scroll" ma-2>
          <v-treeview :items="viewModel.TreeNodes"
            item-key="Name"
            item-text="Name"
            item-children="Children"
            activatable
            open-on-click
            expand-icon>
            <template slot="prepend" slot-scope="{ item, open, leaf }">
              <v-icon v-if="!item.IsMilo" small color="primary">{{open ? 'fa-folder-open' : 'fa-folder'}}</v-icon>
              <v-icon v-else small color="primary">{{fileIcons[item.Type] || 'fa-file'}}</v-icon>
            </template>
          </v-treeview>
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
          Tex: 'fa-file-image'
        }
      }
    },
    computed: {
      //FilteredEntries: function
    },
    mounted: function () {

    }
  }
</script>

<style scoped>
.text {
  text-transform: none;
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
