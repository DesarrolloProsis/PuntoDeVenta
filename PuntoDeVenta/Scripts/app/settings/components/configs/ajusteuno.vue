<template>
    <b-container fluid>
        <h3>Eliminar usuarios del punto de venta</h3>
        <b-alert variant="danger" show><strong>Ojo!</strong> Verificar correctamente el usuario a eliminar.</b-alert>
        <b-table :items="myProvider" :busy="isBusy" class="mt-3" :fields="fields">
            <div slot="table-busy" class="text-center text-danger my-2">
                <b-spinner class="align-middle"></b-spinner>
                <strong>Loading...</strong>
            </div>
            <template slot="actions" slot-scope="row">
                <b-button size="sm" ref="btnShow" @click="info(row.item)" class="mr-1">
                    Eliminar
                </b-button>
            </template>
        </b-table>

        <b-modal :id="infoModal.id" :title="infoModal.title" hide-footer @hide="resetInfoModal">
            <pre>{{infoModal.content}}</pre>
            <b-button class="mt-3" @click="deleteUser(infoModal.idUser)" block>Eliminar</b-button>
        </b-modal>
    </b-container>
</template>

<script>
    import axios from 'axios'

    export default {
        computed: {
            fields() {
                let fields = [{ key: 'Email', label: 'Email' }, { key: 'Role', label: 'Role' }, { key: 'actions', label: 'Actions' }]
                return fields
            }
        },
        data() {
            return {
                isBusy: false,
                infoModal: {
                    id: 'info-modal',
                    title: '',
                    content: '',
                    idUser: ''
                }
            }
        },
        methods: {
            myProvider() {
                // Here we don't set isBusy prop, so busy state will be
                // handled by table itself
                this.isBusy = true
                let promise = axios.get('/Home/GetAllUsers')

                return promise.then((x) => {
                    const items = x.data
                    // Here we could override the busy state, setting isBusy to false
                    this.isBusy = false
                    return (items)
                }).catch(error => {
                    // Here we could override the busy state, setting isBusy to false
                    this.isBusy = false
                    // Returning an empty array, allows table to correctly handle
                    // internal busy state in case of error
                    console.log(error);
                    return []
                })
            },
            info(item) {
                this.infoModal.content = JSON.stringify(item, null, 2)
                this.infoModal.idUser = item.Id
                this.infoModal.title = `Usuario: ${item.Email}`
                this.$root.$emit('bv::show::modal', this.infoModal.id, '#btnShow')
            },
            resetInfoModal() {
                this.infoModal.title = ''
                this.infoModal.content = ''
            },
            deleteUser(id) {
                axios.post('/Home/DeleteUser', {
                    "uid": id
                }).then((x) => {
                    const items = x.data
                    if (items.success != "") {
                        this.$root.$emit('bv::hide::modal', this.infoModal.id, null)
                    }
                }).catch(error => {
                    console.log(error);
                    return []
                })
            }
        }
    }
</script>