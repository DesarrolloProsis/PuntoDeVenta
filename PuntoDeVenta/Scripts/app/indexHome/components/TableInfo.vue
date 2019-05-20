<template>
    <div>
        <div class="py-2">
            <div class="card shadow">
                <div class="card-header">
                    <div class=" row justify-content-center">
                        <div class="col-md-6">
                            <div class="input-group">
                                <div class="input-group-prepend">
                                    <b-form-select v-model="selected" :options="options" @change="onchange()"></b-form-select>
                                </div>
                                <b-form-input v-model="numero" placeholder="Ej. 501000000000, 19012563, etc."></b-form-input>
                                <div class="input-group-append">
                                    <button class="btn btn-buscar" type="button" id="button-addon2" @click="sendToServe"><i class="fas fa-search"></i> Buscar</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <template v-if="mostrar">
                            <b-table :items="items" :fields="fieldsCuenta" :busy="isBusy" striped hover small>
                                <div slot="table-busy" class="text-center text-danger my-2">
                                    <b-spinner class="align-middle"></b-spinner>
                                    <strong>Cargando...</strong>
                                </div>
                                <template slot="Detalles" slot-scope="row">
                                    <b-button size="sm" @click="row.toggleDetails" class="mr-2">
                                        {{ row.detailsShowing ? 'Enconder' : 'Mostrar'}} Detalles
                                    </b-button>
                                </template>

                                <template slot="row-details" slot-scope="row">
                                    <b-card class="shadow-lg">
                                        <div v-for="value in row.item.Tags">
                                            <b-row class="mb-2" style="background-color: #EAFAF1">
                                                <b-col sm="3" class="text-sm-right"><b>Número Tag:</b></b-col>
                                                <b-col>{{ value.NumTag }}</b-col>
                                            </b-row>

                                            <b-row class="mb-2">
                                                <b-col sm="3" class="text-sm-right"><b>Saldo de tag:</b></b-col>
                                                <b-col>{{ value.SaldoTag }}</b-col>
                                            </b-row>

                                            <b-row class="mb-2">
                                                <b-col sm="3" class="text-sm-right"><b>Estatus tag:</b></b-col>
                                                <b-col>{{ value.StatusTag }}</b-col>
                                            </b-row>

                                        </div>
                                    </b-card>
                                </template>
                            </b-table>
                        </template>
                        <template v-else>
                            <b-table :items="items" :fields="fieldsTag" :busy="isBusy" striped hover small>
                                <div slot="table-busy" class="text-center text-danger my-2">
                                    <b-spinner class="align-middle"></b-spinner>
                                    <strong>Cargando...</strong>
                                </div>
                                <template slot="Detalles" slot-scope="row">
                                    <b-button size="sm" @click="row.toggleDetails" class="mr-2">
                                        {{ row.detailsShowing ? 'Esconder' : 'Mostrar'}} Detalles
                                    </b-button>
                                </template>

                                <template slot="row-details" slot-scope="row">
                                    <b-card class="shadow-lg">
                                        <b-row class="mb-2" style="background-color: #EAFAF1">
                                            <b-col sm="3" class="text-sm-right"><b>Nombre cliente:</b></b-col>
                                            <b-col>{{ row.item.NombreCompleto }}</b-col>
                                        </b-row>
                                        <hr />
                                        <b-row class="mb-2">
                                            <b-col sm="3" class="text-sm-right"><b>Número cuenta:</b></b-col>
                                            <b-col>{{ row.item.NumCuenta }}</b-col>
                                        </b-row>
                                        <hr />
                                        <b-row class="mb-2">
                                            <b-col sm="3" class="text-sm-right"><b>Tipo cuenta:</b></b-col>
                                            <b-col>{{ row.item.TypeCuenta }}</b-col>
                                        </b-row>
                                        <hr />
                                    </b-card>
                                </template>
                            </b-table>
                        </template>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script>
    import axios from 'axios';

    export default {
        name: 'TableInfo',
        computed: {
            items() {
                this.mostrar = this.selected == 'Cuenta' ? true : false
                return this.dataArray
            }
        },
        data() {
            return {
                isBusy: false,
                fieldsCuenta: {
                    NombreCompleto: {
                        label: 'Nombre Completo',
                        sortable: true
                    },
                    NumCuenta: {
                        label: 'Número de cuenta',
                        sortable: true
                    },
                    SaldoCuenta: {
                        label: 'Saldo cuenta',
                        sortable: true
                    },
                    TypeCuenta: {
                        label: 'Tipo cuenta',
                        sortable: true
                    },
                    StatusCuenta: {
                        label: 'Estatus cuenta',
                        sortable: true
                    },
                    Detalles: {
                        label: 'Detalles',
                        sortable: false
                    },
                },
                fieldsTag: {
                    NumTag: {
                        label: 'Número tag',
                        sortable: true
                    },
                    SaldoTag: {
                        label: 'Saldo tag',
                        sortable: true
                    },
                    StatusTag: {
                        label: 'Estatus tag',
                        sortable: true
                    },
                    Detalles: {
                        label: 'Detalles',
                        sortable: false
                    },
                },
                selected: 'Cuenta',
                options: [
                    { value: 'Cuenta', text: 'Cuentas' },
                    { value: 'Tag', text: 'Tags' },
                ],
                numero: null,
                dataArray: [],
                mostrar: true,
            }
        },
        methods: {
            sendToServe() {
                this.isBusy = !this.isBusy

                axios.post('/Home/GetInfo', {
                    "Numero": this.numero,
                    "Type": this.selected
                }).then((x) => {
                    // Here we could override the busy state, setting isBusy to false
                    // this.isBusy = false

                    this.dataArray = x.data
                    //console.log(this.dataArray)
                    this.isBusy = !this.isBusy
                }).catch(error => {
                    // Here we could override the busy state, setting isBusy to false
                    // this.isBusy = false
                    // Returning an empty array, allows table to correctly handle
                    // internal busy state in case of error
                    // <b-button @click="toggleBusy">Toggle Busy State</b-button>

                    this.dataArray = []
                    console.error(error)
                })
            },
            onchange() {
                this.numero = null
                this.dataArray = []
            }
        }
    }
</script>
