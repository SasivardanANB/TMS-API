import React from 'react';
import './userapplication.scss';
import classNames from 'classnames/bind';

import Form from '../../../common/form';
import CustomModal from '../../../common/custommodal';

let
    serachFormElems = [
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
                    regex: /^[a-zA-Z0-9]{3,30}$/,
                    message: "Username should not have any special characters, spaces"
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
                    regex: /^[a-zA-Z0-9]{3,30}$/,
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
                    regex: /^[a-zA-Z0-9]{3,30}$/,
                    message: "Password should not have any special characters, spaces"
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
                options: applicationsList.map(x => {
                    let y = Object.assign({}, x, { checked: false })
                    delete y.index;
                    return y
                })
            },
            gridClass: "col-12"
        }
    ],
    users = [
        {
            ID: 1,
            UserName: "JohnDoe",
            Password: "password",
            FirstName: "John",
            LastName: "Doe",
            RoleID: 0,
            BusinessAreaID: 0,
            Applications: [
                0
            ],
            IsActive: true
        },
        {
            ID: 2,
            UserName: "LiAng",
            Password: "password",
            FirstName: "Li",
            LastName: "Ang",
            RoleID: 0,
            BusinessAreaID: 0,
            Applications: [
                1, 2
            ],
            IsActive: true
        }
    ];

class UserApplication extends React.Component {
    constructor(props) {
        super(props);
        this.state = { showModal: false, userList: [], modalForm: JSON.parse(JSON.stringify(modalFormElems)) };
    }

    componentDidMount() {
        this.getUsers();
        //     this.refs.formRef.onFormSubmit()
        //     ref="formRef"
    }

    searchFormSubmit(data) {
        console.log(data);
    }

    modalFormSubmit(data) {
        console.log(data);
    }

    getUsers() {
        setTimeout(() => this.setState({ userList: users }), 1500)
    }

    editUser(x) {
        let y = users.find(user => user.ID === x.ID),
            obj = JSON.parse(JSON.stringify(modalFormElems));

        obj[0].value = y.UserName;
        obj[0].valid = true;
        obj[0].disabled = true;
        obj[2].field = {
            type: "checkbox",
            horizontal: false,
            options: applicationsList.map(v => {
                let w = Object.assign({}, v, { checked: (y.Applications.indexOf(v.index) !== -1) })
                delete w.index;
                return w
            })
        };

        this.setState({ modalForm: obj, showModal: true });
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
                    <h5 className="px-2 font-weight-bold table-heading m-0">User Application List</h5>
                    <button className="btn btn-outline-primary add-button p-2 ml-auto" onClick={() => this.setState({ showModal: true, modalForm: JSON.parse(JSON.stringify(modalFormElems)) })}><i className="fas fa-plus"></i></button>
                </div>

                <div className="table-cover px-2 mt-4">
                    <table className="table">
                        <thead>
                            <tr>
                                <th scope="col"></th>
                                <th scope="col">Username</th>
                                <th scope="col">Application</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                this.state.userList.map((x, i) =>
                                    <tr>
                                        <td className={classNames("row-actions d-flex", { "border-top-0": !i })}>
                                            <i className="far fa-trash-alt user-delete mr-3"></i>
                                            <i className="far fa-edit user-edit" onClick={() => this.editUser(x)}></i>
                                        </td>
                                        <td>{x.UserName}</td>
                                        <td>{
                                            x.Applications.map((y, j) => { return (((j > 0) ? ", " : "") + applicationsList.find(z => z.index === y).label) })
                                        }</td>
                                    </tr>
                                )
                            }
                            {
                                !this.state.userList.length &&
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

export default UserApplication;