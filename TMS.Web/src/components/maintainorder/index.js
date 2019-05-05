import React from 'react';
import './maintainorder.scss';
import classNames from 'classnames/bind';

import ToggleBound from '../common/togglebound';
import Form from '../common/form';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

let 
    uploadFormElems = [
        {
            name: 'Search Order',
            placeholder: 'Search order by order no, packing sheet, police no.',
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
            OrderNo: "SHP-2018-06-26-000141",
            Name1: "Plan Dawuan",
            Name2: "Gudang Madukoro",
            VType: "TR-002",
            EName: "Parani",
            NPolicy: "B 1232 KAG",
            Status: "BOOKED",
        },
        
    ];

class MaintainOrder extends React.Component {
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
                        <div className="tabs-title d-none d-md-block d-lg-block active">Maintain Order</div>
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
                            <h5 className="px-2 font-weight-bold table-heading m-0">Order List</h5>
                            <button type="button" className="btn btn-primary btn-sm ml-2 px-3 search-button ml-auto">UPLOAD ORDER</button>
                            <button className="btn btn-outline-primary add-button p-2 ml-3"><i className="fas fa-plus"></i></button>
                        </div>

                        <div className="table-cover table-responsive px-2 mt-4">
                            <table className="table">
                                <thead>
                                    <tr>
                                        <th scope="col"></th>
                                        <th scope="col">No. Order</th>
                                        <th scope="col">Asal Pengiriman</th>
                                        <th scope="col">Tujuan Pengiriman</th>
                                        <th scope="col">Vehicle Type</th>
                                        <th scope="col">Expedition Name</th>
                                        <th scope="col">Nomor Polosi</th>
                                        <th scope="col">Status Order</th>

                                    </tr>
                                </thead>
                                <tbody>
                                    {
                                        this.state.roleList.map((x, i) =>
                                            <tr key={x.ID}>
                                                <td className={classNames("row-actions d-flex align-items-center", { "border-top-0": !i })}>
                                                    <button type="button" className="btn rounded-circle circular-icon d-flex align-items-center justify-content-center">
                                                        <i className="far fa-trash-alt role-delete"></i>
                                                    </button>
                                                    <button type="button" className="btn ml-2 rounded-circle circular-icon d-flex align-items-center justify-content-center">
                                                        <i className="fas fa-pencil-alt role-delete"></i>
                                                    </button>
                                                    <button type="button" className="btn btn-primary btn-sm ml-2 px-3 search-button w-120">Track Order</button>
                                                </td>
                                                <td>{x.OrderNo}</td>
                                                <td>{x.Name1}</td>
                                                <td>{x.Name2}</td>
                                                <td>{x.VType}</td>
                                                <td>{x.EName}</td>
                                                <td>{x.NPolicy}</td>
                                                <td>{x.Status}</td>
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

export default MaintainOrder;