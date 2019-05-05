import React from 'react';
import './maintainexpeditor.scss';
import classNames from 'classnames/bind';

import ToggleBound from '../common/togglebound';
import Form from '../common/form';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

let 
    uploadFormElems = [
        {
            name: 'Search Expeditor',
            placeholder: 'Search expeditor',
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
                    regex: /^.{3,30}$/,
                    message: "Should be 3 - 30 characters"
                },
                {
                    regex: /^[a-zA-Z]{3,30}$/,
                    message: "First name should not have any special characters, numerics or spaces"
                }
            ]
        }
    ],
    roles = [
        {
            ID: 1,
            Name: "Parani",
            Address: "Jalan Terus Maju",
            City: "Jakarta Utara",
            PIC: "Willy",
            Phone: "081377382101",
        },
        
    ];

class MaintainExpeditor extends React.Component {
    constructor(props) {
        super(props);
        this.state = { inbound: true, roleList: [], startDate: new Date() }
    }

    handleChange(date) {
        this.setState({
            startDate: date
        });
    }

    modalFormSubmit(data) {
        console.log(data);
    }

    componentDidMount() {
        this.getRoles();
    }

    getRoles() {
        setTimeout(() => this.setState({ roleList: roles }), 1500)
    }

    render() {
        return (
            <React.Fragment>
                <div className="text-right">
                    <ToggleBound toggle={this.state.inbound} onClick={() => this.setState({ inbound: !this.state.inbound })} />
                </div>
                <div className="tabs-wrap">
                    <div className="tabs-header-wrap">
                        <div className="tabs-title d-none d-md-block d-lg-block active">Maintain Expenditor</div>
                        <div className="clearfix"></div>
                    </div>
                    <div className="tabs-content">
                      
                        <Form
                            fields={uploadFormElems}
                            className="upload-form px-2"
                            footerClassName="col-12 col-md-6 col-lg-6 d-flex align-items-end"
                            formButtons={<button className="text-uppercase btn btn-primary search-button px-5 mt-0 mb-4" type="submit">Search</button>}
                            onSubmit={obj => this.modalFormSubmit(obj)}
                            ref="formRef"
                        />

                        <div className="table-header-block d-flex mt-4 align-items-center">
                            <h5 className="px-2 font-weight-bold table-heading m-0">Expeditor List</h5>
                            <button className="btn btn-outline-primary add-button p-2 ml-auto"><i className="fas fa-plus"></i></button>
                        </div>

                        <div className="table-cover table-responsive px-2 mt-4">
                            <table className="table">
                                <thead>
                                    <tr>
                                        <th scope="col"></th>
                                        <th scope="col">Name Expeditor</th>
                                        <th scope="col">Alamat Expeditor</th>
                                        <th scope="col">Kota/Kabupaten</th>
                                        <th scope="col">PIC</th>
                                        <th scope="col">Nomor Telepon</th>

                                    </tr>
                                </thead>
                                <tbody>
                                    {
                                        this.state.roleList.map((x, i) =>
                                            <tr key={x.ID}>
                                                <td className={classNames("row-actions d-flex align-items-center", { "border-top-0": !i })}>
                                                    <button type="button" className="btn rounded-circle circular-icon d-flex align-items-center justify-content-center">
                                                        <i className="fas fa-eye role-delete text-secondary"></i>
                                                    </button>
                                                    <button type="button" className="btn ml-2 rounded-circle circular-icon d-flex align-items-center justify-content-center">
                                                        <i className="fas fa-pencil-alt role-delete text-secondary"></i>
                                                    </button>
                                                </td>
                                                <td>{x.Name}</td>
                                                <td>{x.Address}</td>
                                                <td>{x.City}</td>
                                                <td>{x.PIC}</td>
                                                <td>{x.Phone}</td>
                                            </tr>
                                        )
                                    }
                                    {
                                        !this.state.roleList.length &&
                                        <tr>
                                            <td colspan="3" className="text-center">
                                                <img width="auto" height="80px" src={require("../../img/loader.svg")} />
                                            </td>
                                        </tr>
                                    }
                                </tbody>
                            </table>
                            
                        </div>
                        <div className="pagination-wrap mt-3">
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
                </div>
            </React.Fragment>
        );
    }
}

export default MaintainExpeditor;