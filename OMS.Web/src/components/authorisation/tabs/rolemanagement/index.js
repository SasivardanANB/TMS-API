import React from 'react';
import './rolemanagement.scss';
import classNames from 'classnames/bind';
import axios from 'axios';
import { toast } from 'react-toastify';
import { rootURL, ops } from '../../../../config';

import Form from '../../../common/form';
import CustomModal from '../../../common/custommodal';
import Pagination from '../../../common/pagination';
import PageSizeSelector from '../../../common/pagesizeselector';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { addloader, removeloader, modifyerror } from '../../../../store/actions';
import { getroles, getmenuactivities } from '../../../../store/viewsactions/authorisation';

let
    serachFormElems = [
        {
            name: 'Role Code',
            placeholder: 'Search role code',
            value: '',
            errMsg: '',
            required: true,
            valid: false,
            field: {
                type: "text"
            },
            gridClass: "col-12 col-md-6 col-lg-6",
            check: [
                {
                    regex: /^.{2,}$/,
                    message: "Should be atleast 2 characters"
                },
                {
                    regex: /^[a-zA-Z0-9]{2,}$/,
                    message: "Role code should not have any special characters, spaces"
                }
            ]
        }
    ],
    modalFormElems = [
        {
            name: 'Role Code',
            placeholder: 'Insert role code',
            value: '',
            errMsg: '',
            required: true,
            valid: false,
            field: {
                type: "text"
            },
            gridClass: "col-12",
            check: [
                {
                    regex: /^.{2,}$/,
                    message: "Role code should be atleast 2 characters"
                },
                {
                    regex: /^[a-zA-Z0-9]{2,}$/,
                    message: "Role code should not have any special characters, spaces"
                }
            ]
        },
        {
            name: 'Role Description',
            placeholder: 'Insert role description',
            value: '',
            errMsg: '',
            required: true,
            valid: false,
            field: {
                type: "text"
            },
            gridClass: "col-12",
            check: [
                {
                    regex: /^.{2,}$/,
                    message: "Role description should be atleast 2 characters"
                },
                {
                    regex: /^[a-zA-Z0-9]{2,}$/,
                    message: "Role description should not have any special characters, spaces"
                }
            ]
        }
    ];

class RoleManagement extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            showModal: false,
            modalForm: JSON.parse(JSON.stringify(modalFormElems)),
            SortOrder: "rolecode",
            PageSize: 10,
            PageNumber: 1,
            keyword: "",
            menuActivities: [],
            editId: null
        };
        this.getRoles();
        props.getmenuactivities();
    }

    async searchFormSubmit(data) {
        await this.setState({ keyword: data[0].value, PageNumber: 1 });
        this.props.getroles({
            "Requests": [
                {
                    "isActive": true,
                    "RoleCode": this.state.keyword
                }
            ],
            "SortOrder": this.state.SortOrder,
            "PageSize": this.state.PageSize,
            "PageNumber": this.state.PageNumber
        });
        this.refs.searchFormRef.switchFormSubmit(false);
    }

    resetSearch() {
        this.setState({ keyword: "" });
        this.getRoles();
    }

    addRoleManagement() {
        this.setState({ showModal: true, editId: null, menuActivities: this.props.menuActivities, modalForm: JSON.parse(JSON.stringify(modalFormElems)) });
    }

    async editRoleManagement(id){
        console.log(id);
        let self = this;
        this.props.addloader('editRoleManagement');

        await axios(rootURL + ops.users.GetRoleDetails + "?roleId=" + id, {
            method: 'GET',
            headers: {
                "Content-Type": "application/json",
                "Token": this.props.credentials.token
            }
        })
            .then(function (response) {
                if (response.statusText === "OK") {
                    console.log("response", response);
                    if (response.data.StatusCode === 200) {
                        let userMenu = response.data.Data[0].RoleMenus,
                            newMenu = self.props.menuActivities.map(x => {
                                let thisUserMenu = userMenu.find(y => y.MenuCode === x.MenuCode);
                                if(thisUserMenu){
                                    x.checked = true;
                                    if(thisUserMenu.RoleMenuActivities.length){
                                        x.RoleMenuActivities.map(z => {
                                            let thisUserActivity = thisUserMenu.RoleMenuActivities.find(w => w.ActivityCode === z.ActivityCode);
                                            if(thisUserActivity){
                                                console.log(z.ActivityCode);
                                                z.checked = true;
                                            }
                                            else{
                                                z.checked = false;                                                
                                            }
                                            return z;
                                        })
                                    }
                                }
                                else{
                                    x.checked = false;
                                }
                                return x;
                            })

                            let thisRole = self.props.rolesMngtTable.Data.find(x => x.ID === id),
                                modalForm = JSON.parse(JSON.stringify(modalFormElems));
                                modalForm[0].value = thisRole.RoleCode;
                                modalForm[0].valid = true;
                                modalForm[0].disabled = true;
                                modalForm[1].value = thisRole.RoleDescription;
                                modalForm[1].valid = true;

                            self.setState({ showModal: true, editId: id, menuActivities: newMenu, modalForm: modalForm });                        

                    }
                    else {
                        console.log("response", response);
                        self.props.modifyerror({ show: true });
                    }
                }
                else {
                    console.log("response", response);
                    self.props.modifyerror({ show: true });
                }
            })
            .catch(function (error) {
                self.props.modifyerror({ show: true });
                console.log("error", error)
            });

        this.props.removeloader('editRoleManagement');
    }

    handleRoleSelection(val, ...id) {
        let newmenuActivities = [];
        if (id.length === 1) {
            newmenuActivities = JSON.parse(JSON.stringify(this.state.menuActivities)).map(x => {
                let returnObj = (x.ID === id[0]) ? Object.assign({}, x, { checked: val }) : x;
                if (x.ID === id[0] && returnObj.RoleMenuActivities.length) {
                    returnObj.RoleMenuActivities.map(y => {
                        y.checked = false
                        if (y.ID === 1) {
                            y.checked = val
                        }
                        return y
                    })
                }
                return returnObj;
            });
        }
        else {
            if (id[1] === 1) {
                newmenuActivities = JSON.parse(JSON.stringify(this.state.menuActivities)).map(x => {
                    let returnObj = (x.ID === id[0]) ? Object.assign({}, x, { checked: val }) : x;
                    if (x.ID === id[0] && returnObj.RoleMenuActivities.length) {
                        returnObj.RoleMenuActivities.map(y => {
                            y.checked = false
                            if (y.ID === 1) {
                                y.checked = val
                            }
                            return y
                        })
                    }
                    return returnObj;
                });
            }
            else {
                newmenuActivities = JSON.parse(JSON.stringify(this.state.menuActivities)).map(x => {
                    if (x.ID === id[0]) {
                        let returnObj = Object.assign({}, x, { checked: true });
                        returnObj.RoleMenuActivities.map(y => {
                            if (val && y.ID === 1) {
                                y.checked = true
                            }
                            else if (y.ID === id[1]) {
                                y.checked = val
                            }
                            return y
                        });
                        return returnObj
                    }
                    return x
                });
            }
        }
        this.setState({ menuActivities: newmenuActivities });
    }

    async modalFormSubmit(data) {
        let self = this;
        this.props.addloader('roleMngtFormSubmit');

        let body = {
            "Requests":[
                {
                    "RoleCode": data[0].value,
                    "RoleDescription": data[1].value,
                    "ValidFrom": "2019-04-30",
                    "ValidTo": "2019-05-30",
                    "IsActive": true,
                    "RoleMenus": this.state.menuActivities.filter(x => x.checked).map(x => {
                        let menu = {};
                        menu.ID = x.ID;
                        menu.RoleMenuActivities = x.RoleMenuActivities.filter(y => y.checked).map(y => {return {ID: y.ID}})
                        return menu
                    })
                }
            ],
            "CreatedBy": "system"
        };
        console.log(body);

        if (this.state.editId !== null) {
            body.Requests[0].ID = this.state.editId.toString(10);
        }

        await axios(rootURL + ops.users.createupdaterole, {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "Token": this.props.credentials.token
            },
            data: JSON.stringify(body)
        })
            .then(function (response) {
                if (response.statusText === "OK") {
                    console.log("response", response);
                    if (response.data.StatusCode === 200) {
                        toast.success(response.data.StatusMessage);
                        self.getRoles();
                        // success toast
                    }
                    else {
                        toast.error(response.data.StatusMessage);
                        // error toast
                    }
                }
                else {
                    console.log("response", response);
                    self.props.modifyerror({ show: true });
                }
            })
            .catch(function (error) {
                self.props.modifyerror({ show: true });
                console.log("error", error)
            });

        this.setState({ showModal: false });
        this.props.removeloader('roleMngtFormSubmit');
    }

    async deleteRole(id){
        console.log(id);
        let self = this;
        this.props.addloader('deleteRole');

        await axios(rootURL + ops.users.deleterole + "?id=" + id, {
            method: 'DELETE',
            headers: {
                "Content-Type": "application/json",
                "Token": this.props.credentials.token
            }
        })
            .then(function (response) {
                if (response.statusText === "OK") {
                    console.log("response", response);
                    if (response.data.StatusCode === 200) {
                        toast.success(response.data.StatusMessage);
                        self.getRoles();
                        // success toast
                    }
                    else {
                        toast.error(response.data.StatusMessage);
                        // error toast
                    }
                }
                else {
                    console.log("response", response);
                    self.props.modifyerror({ show: true });
                }
            })
            .catch(function (error) {
                self.props.modifyerror({ show: true });
                console.log("error", error)
            });

        this.props.removeloader('deleteRole');

    }

    async paginate(i) {
        await this.setState({ PageNumber: i });
        this.getRoles();
    }

    async sortTable(i) {
        await this.setState({ SortOrder: i });
        this.getRoles();
    }

    async rowsPerPageChange(i) {
        await this.setState({ PageSize: i, PageNumber: 1 });
        this.getRoles();
    }

    getRoles() {
        this.props.getroles({
            "Requests": [
                {
                    "isActive": true
                }
            ],
            "SortOrder": this.state.SortOrder,
            "PageSize": this.state.PageSize,
            "PageNumber": this.state.PageNumber
        });
    }

    render() {
        let searchFormButtons = <React.Fragment>
            <button className="text-uppercase btn btn-primary submit-button px-5 mt-0" type="submit">Search</button>
            {
                this.state.keyword &&
                <button className="btn btn-outline-danger reset-button p-2 ml-3" onClick={() => this.resetSearch()}>{this.state.keyword}<i className="fas fa-times ml-2"></i></button>
            }
        </React.Fragment>,
            modalFormButtons = <React.Fragment>
                <button className="text-uppercase btn btn-primary save-button px-5 mt-0 ml-auto" onClick={() => this.refs.formRef.onFormSubmit()}>SAVE</button>
                <button className="text-uppercase btn btn-primary cancel-button px-5 mt-0 ml-3" onClick={() => this.setState({ showModal: false })}>CANCEL</button>
            </React.Fragment>;
        return (
            <React.Fragment>
                <Form
                    fields={serachFormElems}
                    className="search-form px-2"
                    footerClassName="col-12 col-md-6 col-lg-6 d-flex"
                    formButtons={searchFormButtons}
                    onSubmit={obj => this.searchFormSubmit(obj)}
                    ref="searchFormRef"
                />

                <div className="table-header-block d-flex mt-4 align-items-center">
                    <h5 className="px-2 font-weight-bold table-heading m-0">Role Management List</h5>
                    <button className="btn btn-outline-primary add-button p-2 ml-auto" onClick={() => this.addRoleManagement()}><i className="fas fa-plus"></i></button>
                </div>

                <PageSizeSelector NumberOfRecords={this.props.rolesMngtTable.NumberOfRecords} value={this.state.PageSize} onChange={x => this.rowsPerPageChange(x)} />

                <div className="table-cover px-2 mt-4">
                    <table className="table">
                        <thead>
                            <tr>
                                <th scope="col"></th>
                                <th scope="col" style={{ "cursor": "pointer" }} onClick={() => this.sortTable((this.state.SortOrder === "rolecode") ? "rolecode_desc" : "rolecode")}>{(this.state.SortOrder.indexOf("rolecode") !== -1) && <i className={"mr-2 fas fa-sort-" + ((this.state.SortOrder === "rolecode") ? "down" : "up")}></i>}Role Code</th>
                                <th scope="col" style={{ "cursor": "pointer" }} onClick={() => this.sortTable((this.state.SortOrder === "roledescription") ? "roledescription_desc" : "roledescription")}>{(this.state.SortOrder.indexOf("roledescription") !== -1) && <i className={"mr-2 fas fa-sort-" + ((this.state.SortOrder === "roledescription") ? "down" : "up")}></i>}Role Description</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                Boolean(this.props.rolesMngtTable.NumberOfRecords) ?
                                    this.props.rolesMngtTable.Data.map((x, i) =>
                                        <tr key={x.ID}>
                                            <td className={classNames("row-actions d-flex", { "border-top-0": !i })}>
                                                <i className="far fa-trash-alt role-delete mr-3" onClick={() => this.deleteRole(x.ID)}></i>
                                                <i className="far fa-edit role-edit" onClick={() => this.editRoleManagement(x.ID)}></i>
                                            </td>
                                            <td>{x.RoleCode}</td>
                                            <td>{x.RoleDescription}</td>
                                        </tr>
                                    ) :
                                    <tr><td className="text-center" colSpan="3">No records found</td></tr>
                            }
                        </tbody>
                    </table>
                    <Pagination PageSize={this.state.PageSize} PageNumber={this.state.PageNumber} NumberOfRecords={this.props.rolesMngtTable.NumberOfRecords} onClick={i => this.paginate(i)} />
                </div>

                <CustomModal modaltitle="Add New Role Management" isOpen={this.state.showModal} onClick={() => this.setState({ showModal: false })}>
                    <Form
                        className="px-2"
                        fields={this.state.modalForm}
                        onSubmit={obj => this.modalFormSubmit(obj)}
                        footerClassName="d-none"
                        ref="formRef"
                    />
                    <div className="menu-activity-wrap d-flex flex-column px-2">
                        <div className="d-flex menu-activity-header text-center">
                            <h6 className="flex-grow-1 font-weight-bold">Menu</h6>
                            <h6 className="flex-grow-1 font-weight-bold">Activity</h6>
                        </div>
                        {
                            this.state.menuActivities.map(x =>
                                <div key={x.ID} className="menu-activity-pair row mt-3">
                                    <div className="menu-activity-block col-6">
                                        <div className="form-check">
                                            <input className="form-check-input" type="checkbox" checked={x.checked} onChange={(e) => this.handleRoleSelection(e.target.checked, x.ID)} />
                                            <label className="form-check-label">{x.MenuDescription}</label>
                                        </div>
                                    </div>
                                    <div className="menu-activity-block col-6">
                                        {
                                            x.RoleMenuActivities.map(y =>
                                                <div key={y.ID} className="form-check">
                                                    <input className="form-check-input" type="checkbox" checked={y.checked} onChange={(e) => this.handleRoleSelection(e.target.checked, x.ID, y.ID)} />
                                                    <label className="form-check-label">{y.ActivityDescription}</label>
                                                </div>
                                            )
                                        }
                                    </div>
                                </div>
                            )
                        }
                    </div>
                    <div className="modal-form-footer text-right mt-4">
                        {modalFormButtons}
                    </div>
                </CustomModal>
            </React.Fragment>
        );
    }
}

const mapStateToProps = (state) => {
    let { credentials, views } = state,
        { authorisation } = views,
        { rolesMngtTable, menuActivities } = authorisation;
    return { credentials, rolesMngtTable, menuActivities }
};

const mapDispatchToProps = dispatch => (
    bindActionCreators({
        addloader,
        removeloader,
        modifyerror,
        getroles,
        getmenuactivities
    }, dispatch)
);

export default connect(mapStateToProps, mapDispatchToProps)(RoleManagement);