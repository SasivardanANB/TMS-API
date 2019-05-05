import React from 'react';
import './rolemanagement.scss';
import classNames from 'classnames/bind';

import Form from '../../../common/form';
import CustomModal from '../../../common/custommodal';

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
                    regex: /^[a-zA-Z0-9]{3,30}$/,
                    message: "Role code should not have any special characters, spaces"
                }
            ]
        }
    ],
    applicationsList = [
        {
            label: 'TMS',
            index: 0
        },
        {
            label: 'OMS',
            index: 1
        },
        {
            label: 'DMS',
            index: 2
        }
    ],
    modalFormElems = [
        {
            name: 'Role Code',
            value: '',
            errMsg: '',
            required: true,
            valid: false,
            field: {
                type: "select",
                options: [
                    {
                        label: 'Select role code',
                        value: ''
                    },
                    {
                        label: 'option 1',
                        value: '1'
                    },
                    {
                        label: 'option 2',
                        value: '2'
                    },
                    {
                        label: 'option 3',
                        value: '3'
                    }
                ]
            },
            gridClass: "col-12",
            check: [
                {
                    regex: /^[0-9]$/,
                    message: "Please select an option"
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
                    regex: /^[a-zA-Z0-9]{3,30}$/,
                    message: "Password should not have any special characters, spaces"
                }
            ]
        }
    ],
    roles = [
        {
            ID: 1,
            RoleCode: "Head Office",
            RoleDescription: "Head Office",
            ValidFrom: "2019-04-17T18:50:40.508Z",
            ValidTo: "2019-04-17T18:50:40.508Z"
        },
        {
            ID: 2,
            RoleCode: "Shipper",
            RoleDescription: "Shipper",
            ValidFrom: "2019-04-17T18:50:40.508Z",
            ValidTo: "2019-04-17T18:50:40.508Z"
        }
    ];

class RoleManagement extends React.Component {
    constructor(props) {
        super(props);
        this.state = { showModal: false, roleList: [], modalForm: JSON.parse(JSON.stringify(modalFormElems)) };
    }

    componentDidMount() {
        this.getRoles();
    }

    searchFormSubmit(data) {
        console.log(data);
    }

    modalFormSubmit(data) {
        console.log(data);
    }

    getRoles() {
        setTimeout(() => this.setState({ roleList: roles }), 1500)
    }

    render() {
        let modalFormButtons = <React.Fragment>
            <button className="text-uppercase btn btn-primary save-button px-5 mt-0 ml-auto" onClick={() => this.refs.formRef.onFormSubmit()}>SAVE</button>
            <button className="text-uppercase btn btn-primary cancel-button px-5 mt-0 ml-3" onClick={() => this.setState({ showModal: false })}>CANCEL</button>
        </React.Fragment>;
        return (
            <React.Fragment>
                <Form
                    fields={serachFormElems}
                    className="search-form px-2"
                    footerClassName="col-12 col-md-6 col-lg-6 d-flex"
                    formButtons={<button className="text-uppercase btn btn-primary submit-button px-5 mt-0" type="submit">Search</button>}
                    onSubmit={obj => this.searchFormSubmit(obj)}
                />

                <div className="table-header-block d-flex mt-4 align-items-center">
                    <h5 className="px-2 font-weight-bold table-heading m-0">Role Management List</h5>
                    <button className="btn btn-outline-primary add-button p-2 ml-auto" onClick={() => this.setState({ showModal: true, modalForm: JSON.parse(JSON.stringify(modalFormElems)) })}><i className="fas fa-plus"></i></button>
                </div>

                <div className="table-cover px-2 mt-4">
                    <table className="table">
                        <thead>
                            <tr>
                                <th scope="col"></th>
                                <th scope="col">Role Code</th>
                                <th scope="col">Role</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                this.state.roleList.map((x, i) =>
                                    <tr>
                                        <td className={classNames("row-actions d-flex", { "border-top-0": !i })}>
                                            <button type="button" className="btn mr-2 rounded-circle circular-icon d-flex align-items-center justify-content-center">
                                                <i className="far fa-trash-alt role-delete"></i>
                                            </button>
                                            <button type="button" className="btn rounded-circle circular-icon d-flex align-items-center justify-content-center">
                                                <i className="far fa-edit role-edit"></i>
                                            </button>
                                        </td>
                                        <td>{x.RoleCode}</td>
                                        <td>{x.RoleDescription}</td>
                                    </tr>
                                )
                            }
                            {
                                !this.state.roleList.length &&
                                <tr>
                                    <td colspan="3" className="text-center">
                                        <img width="auto" height="80px" src={require("../../../../img/loader.svg")} />
                                    </td>
                                </tr>
                            }
                        </tbody>
                    </table>
                    <div className="pagination-wrap">
                        <nav>
                            <ul className="pagination">
                                <li className="page-item disabled ml-auto">
                                    <a className="page-link" tabindex="-1" aria-disabled="true">PREV</a>
                                </li>
                                <li className="page-item active" aria-current="page"><a className="page-link">1</a></li>
                                <li className="page-item"><a className="page-link">2</a></li>
                                <li className="page-item"><a className="page-link">3</a></li>
                                <li className="page-item">
                                    <a className="page-link">NEXT</a>
                                </li>
                            </ul>
                        </nav>
                    </div>
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
                        <div class="d-flex menu-activity-header text-center">
                            <h6 class="flex-grow-1 font-weight-bold">Menu</h6>
                            <h6 class="flex-grow-1 font-weight-bold">Activity</h6>
                        </div>
                        <div className="menu-activity-pair row mt-3">
                            <div className="menu-activity-block col-6">
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" />
                                    <label class="form-check-label">Upload Data</label>
                                </div>
                            </div>
                            <div className="menu-activity-block col-6">
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" />
                                    <label class="form-check-label">View</label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" />
                                    <label class="form-check-label">Create Master Data</label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" />
                                    <label class="form-check-label">Edit Master Data</label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" />
                                    <label class="form-check-label">Delete Master Data</label>
                                </div>
                            </div>
                        </div>
                        <div className="menu-activity-pair row mt-3">
                            <div className="menu-activity-block col-6">
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" />
                                    <label class="form-check-label">Download Data</label>
                                </div>
                            </div>
                            <div className="menu-activity-block col-6">
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" />
                                    <label class="form-check-label">View</label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" />
                                    <label class="form-check-label">Create Master Data</label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" />
                                    <label class="form-check-label">Edit Master Data</label>
                                </div>
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" />
                                    <label class="form-check-label">Delete Master Data</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="modal-form-footer text-right mt-4">
                        {modalFormButtons}
                    </div>
                </CustomModal>
            </React.Fragment>
        );
    }
}

export default RoleManagement;