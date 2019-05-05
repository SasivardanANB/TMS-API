import React from 'react';
import './userrole.scss';
import classNames from 'classnames/bind';

import Form from '../../../common/form';
import CustomModal from '../../../common/custommodal';

let
    serachFormElems = [
        {
            name: 'Region Code',
            placeholder: 'Search region code',
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
                    message: "Region code should not have any special characters, spaces"
                }
            ]
        }
    ],
    modalFormElems = [
        {
            name: 'Username',
            value: '',
            errMsg: '',
            required: true,
            valid: false,
            field: {
                type: "select",
                options: [
                    {
                        label: 'Select username',
                        value: ''
                    },
                    {
                        label: 'User 1',
                        value: '1'
                    },
                    {
                        label: 'User 2',
                        value: '2'
                    },
                    {
                        label: 'User 3',
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
            name: 'Role',
            value: '',
            errMsg: '',
            required: true,
            valid: false,
            field: {
                type: "select",
                options: [
                    {
                        label: 'Select Role',
                        value: ''
                    },
                    {
                        label: 'Role 1',
                        value: '1'
                    },
                    {
                        label: 'Role 2',
                        value: '2'
                    },
                    {
                        label: 'Role 3',
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
            name: 'Region',
            value: '',
            errMsg: '',
            required: true,
            valid: false,
            field: {
                type: "select",
                options: [
                    {
                        label: 'Select region',
                        value: ''
                    },
                    {
                        label: 'Region 1',
                        value: '1'
                    },
                    {
                        label: 'Region 2',
                        value: '2'
                    },
                    {
                        label: 'Region 3',
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
        }
    ],
    roles = [
        {
            ID: 1,
            Username: "Head Office",
            Role: "Head Office",
            Region: "vscscc",
            ValidFrom: "2019-04-17T18:50:40.508Z",
            ValidTo: "2019-04-17T18:50:40.508Z"
        },
        {
            ID: 2,
            Username: "Head Office",
            Role: "Head Office",
            Region: "vscscc",
            ValidFrom: "2019-04-17T18:50:40.508Z",
            ValidTo: "2019-04-17T18:50:40.508Z"
        }
    ];

class UserRole extends React.Component {
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
            <button className="text-uppercase btn btn-primary save-button px-5 mt-0 ml-auto" type="submit">SAVE</button>
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
                    <h5 className="px-2 font-weight-bold table-heading m-0">User Role List</h5>
                    <button className="btn btn-outline-primary add-button p-2 ml-auto" onClick={() => this.setState({ showModal: true, modalForm: JSON.parse(JSON.stringify(modalFormElems)) })}><i className="fas fa-plus"></i></button>
                </div>

                <div className="table-cover px-2 mt-4">
                    <table className="table">
                        <thead>
                            <tr>
                                <th scope="col"></th>
                                <th scope="col">Username</th>
                                <th scope="col">Role</th>
                                <th scope="col">Region</th>
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
                                        <td>{x.Username}</td>
                                        <td>{x.Role}</td>
                                        <td>{x.Region}</td>
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

                <CustomModal modaltitle="Add New User Role" isOpen={this.state.showModal} onClick={() => this.setState({ showModal: false })}>
                    <Form
                        className="px-2"
                        fields={this.state.modalForm}
                        onSubmit={obj => this.modalFormSubmit(obj)}
                        footerClassName="col-12 d-flex modal-form-footer mt-3"
                        formButtons={modalFormButtons}
                    />
                </CustomModal>
            </React.Fragment>
        );
    }
}

export default UserRole;