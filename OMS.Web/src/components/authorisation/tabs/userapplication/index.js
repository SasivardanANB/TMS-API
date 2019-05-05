import React from 'react';
import './userapplication.scss';
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
import { getusers, getapplications } from '../../../../store/viewsactions/authorisation';

let serachFormElems = [
    {
        name: 'Username',
        placeholder: 'Search by username',
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
                message: "Username should not have any special characters, spaces"
            }
        ]
    }
],
    modalFormElems = [
        {
            name: 'Username',
            placeholder: 'Insert username',
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
                    regex: '^.{2,}$',
                    message: "Username should be atleast 2 characters"
                },
                {
                    regex: '^[a-zA-Z0-9]{2,}$',
                    message: "Username should not have any special characters, spaces"
                }
            ]
        },
        {
            name: 'Password',
            placeholder: 'Insert Password',
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
                    regex: '^.{2,}$',
                    message: "Password should be atleast 2 characters"
                },
                {
                    regex: '^[a-zA-Z0-9]{2,}$',
                    message: "Password should not have any special characters, spaces"
                }
            ]
        },
        {
            name: 'Firstname',
            placeholder: 'Insert Firstname',
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
                    regex: '^.{2,}$',
                    message: "Firstname should be atleast 2 characters"
                },
                {
                    regex: '^[a-zA-Z]{2,}$',
                    message: "Firstname should not have any special characters, numbers or spaces"
                }
            ]
        },
        {
            name: 'Lastname',
            placeholder: 'Insert Lastname',
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
                    regex: '^.{2,}$',
                    message: "Lastname should be atleast 2 characters"
                },
                {
                    regex: '^[a-zA-Z]{2,}$',
                    message: "Lastname should not have any special characters, numbers or spaces"
                }
            ]
        },
        {
            name: 'Application',
            value: '',
            errMsg: 'Select atleast one',
            required: true,
            valid: false,
            field: {
                type: "checkbox",
                horizontal: false,
                options: []
            },
            gridClass: "col-12"
        }
    ];

class UserApplication extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            showModal: false,
            modalForm: JSON.parse(JSON.stringify(this.modalFormElems())),
            SortOrder: "username",
            PageSize: 10,
            PageNumber: 1,
            keyword: "",
            editId: null
        };
        this.getUsers();
        props.getapplications();
    }

    async searchFormSubmit(data) {
        await this.setState({ keyword: data[0].value, PageNumber: 1 });
        this.props.getusers({
            "Requests": [
                {
                    "isActive": true,
                    "UserName": this.state.keyword
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
        this.getUsers();
    }

    modalFormElems() {
        let formObj = JSON.parse(JSON.stringify(modalFormElems));
        formObj[4].field.options = this.props.applications.map(x => {
            let y = Object.assign({}, { label: x.ApplicationName, checked: false })
            return y
        });
        return formObj;
    }

    editUser(x) {
        console.log(x);
        let { userAppTable } = this.props,
            y = userAppTable.Data.find(user => user.ID === x.ID),
            obj = JSON.parse(JSON.stringify(this.modalFormElems()));

        obj[0].value = y.UserName;
        obj[0].valid = true;
        obj[0].disabled = true;
        obj[2].value = y.FirstName;
        obj[2].valid = true;
        obj[3].value = y.LastName;
        obj[3].valid = true;
        obj[4].field.options = this.props.applications.map(v => {
            let w = Object.assign({}, { label: v.ApplicationName, checked: (y.Applications.indexOf(v.ID) !== -1) })
            return w
        });

        this.setState({ modalForm: obj, editId: x.ID, showModal: true });
    }

    async modalFormSubmit(data) {
        console.log(data)
        let self = this;
        this.props.addloader('userAppFormSubmit');

        let body = {
            "Requests": [
                {
                    "UserName": data[0].value,
                    "Password": data[1].value,
                    "FirstName": data[2].value,
                    "LastName": data[3].value,
                    "IsActive": true,
                    "Applications": data[4].field.options.filter(x => x.checked).map(x => [this.props.applications.find(y => (y.ApplicationName === x.label))][0].ID)
                }
            ],
            "CreatedBy": "system"
        };

        if (this.state.editId !== null) {
            body.Requests[0].ID = this.state.editId.toString(10);
        }

        console.log(body);

        await axios(rootURL + ops.users.createupdateuser, {
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
                        self.getUsers();
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
        this.props.removeloader('userAppFormSubmit');
    }

    async deleteUserApp(id){
        console.log(id);
        let self = this;
        this.props.addloader('deleteUserApp');

        await axios(rootURL + ops.users.deleteuser + "?userId=" + id, {
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
                        self.getUsers();
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

        this.props.removeloader('deleteUserApp');

    }

    async paginate(i) {
        await this.setState({ PageNumber: i });
        this.getUsers();
    }

    async sortTable(i) {
        await this.setState({ SortOrder: i });
        this.getUsers();
    }

    async rowsPerPageChange(i) {
        await this.setState({ PageSize: i, PageNumber: 1 });
        this.getUsers();
    }

    getUsers() {
        this.props.getusers({
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
                <button className="text-uppercase btn btn-primary save-button px-5 mt-0 ml-auto" type="submit">SAVE</button>
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
                    <h5 className="px-2 font-weight-bold table-heading m-0">User Application List</h5>
                    <button className="btn btn-outline-primary add-button p-2 ml-auto" onClick={() => this.setState({ showModal: true, editId: null, modalForm: JSON.parse(JSON.stringify(this.modalFormElems())) })}><i className="fas fa-plus"></i></button>
                </div>

                <PageSizeSelector NumberOfRecords={this.props.userAppTable.NumberOfRecords} value={this.state.PageSize} onChange={x => this.rowsPerPageChange(x)} />

                <div className="table-cover px-2 mt-4">
                    <table className="table">
                        <thead>
                            <tr>
                                <th scope="col"></th>
                                <th scope="col" style={{ "cursor": "pointer" }} onClick={() => this.sortTable((this.state.SortOrder === "username") ? "username_desc" : "username")}><i className={"mr-2 fas fa-sort-" + ((this.state.SortOrder === "username") ? "down" : "up")}></i>Username</th>
                                <th scope="col">Application</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                Boolean(this.props.userAppTable.NumberOfRecords) ?
                                    this.props.userAppTable.Data.map((x, i) =>
                                        <tr key={x.ID}>
                                            <td className={classNames("row-actions d-flex", { "border-top-0": !i })}>
                                                <i className="far fa-trash-alt user-delete mr-3" onClick={() => this.deleteUserApp(x.ID)}></i>
                                                <i className="far fa-edit user-edit" onClick={() => this.editUser(x)}></i>
                                            </td>
                                            <td>{x.UserName}</td>
                                            <td>{
                                                x.Applications.map((y, j) => { return (((j > 0) ? ", " : "") + this.props.applications.find(z => z.index === y).label) })
                                            }</td>
                                        </tr>
                                    ) :
                                    <tr><td className="text-center" colSpan="3">No records found</td></tr>
                            }
                        </tbody>
                    </table>
                    <Pagination PageSize={this.state.PageSize} PageNumber={this.state.PageNumber} NumberOfRecords={this.props.userAppTable.NumberOfRecords} onClick={i => this.paginate(i)} />
                </div>

                <CustomModal modaltitle="Add New User Application" isOpen={this.state.showModal} onClick={() => this.setState({ showModal: false })}>
                    <Form
                        className="px-2"
                        fields={this.state.modalForm}
                        onSubmit={obj => this.modalFormSubmit(obj)}
                        footerClassName="col-12 d-flex modal-form-footer"
                        formButtons={modalFormButtons}
                    />
                </CustomModal>
            </React.Fragment>
        );
    }
}

const mapStateToProps = (state) => {
    let { credentials, views } = state,
        { authorisation } = views,
        { userAppTable, applications } = authorisation;
    return { credentials, userAppTable, applications }
};

const mapDispatchToProps = dispatch => (
    bindActionCreators({
        addloader,
        removeloader,
        modifyerror,
        getusers,
        getapplications
    }, dispatch)
);

export default connect(mapStateToProps, mapDispatchToProps)(UserApplication);