import React from 'react';
import './tripmanagement.scss';
import classNames from 'classnames/bind';

import ToggleBound from '../common/togglebound';
import Form from '../common/form';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

let 
    uploadFormElems = [
        {
            name: 'Filter by Status Order',
            value: '',
            errMsg: '',
            required: true,
            valid: false,
            field: {
                type: "select",
                options: [
                    {
                        label: 'All',
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
            gridClass: "col-12 col-sm-6 col-md-4 col-lg-6",
            check: [
                {
                    regex: /^[0-9]$/,
                    message: "Please select an option"
                }
            ]
        },
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
            Description: "CD Engkel/10 Palet",
            Dimension: "6 cbm/3000 kg",
            Vehicle: "B 1234 ABC - Truck Load",
            EstimatedTransportTime: "Friday, 1 January 2019",
            EstimatedTransportArrived: "Monday, 4 January 2019",
            Status: "CONFIRM ORDER",
            ValidFrom: "2019-04-17T18:50:40.508Z",
            ValidTo: "2019-04-17T18:50:40.508Z"
        },
        {
            ID: 2,
            OrderNo: "SHP-2018-06-26-000140",
            Name1: "Plan Dawuan",
            Name2: "Gudang Madukoro",
            Description: "CD Engkel/10 Palet",
            Dimension: "6 cbm/3000 kg",
            Vehicle: "B 1234 ABC - Truck Load",
            EstimatedTransportTime: "Friday, 1 January 2019",
            EstimatedTransportArrived: "Monday, 4 January 2019",
            Status: "CONFIRM ORDER",
            ValidFrom: "2019-04-17T18:50:40.508Z",
            ValidTo: "2019-04-17T18:50:40.508Z"
        }
    ];

class TripManagement extends React.Component {
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
                        <div className="tabs-title d-none d-md-block d-lg-block active">Trip Management</div>
                        <div className="clearfix"></div>
                    </div>
                    <div className="tabs-content">
                        <Form
                            fields={uploadFormElems}
                            className="upload-form px-2"
                            footerClassName="col-12"
                            formButtons={<button className="text-uppercase btn btn-primary search-button px-5 mt-0" type="submit">Search</button>}
                            onSubmit={obj => this.modalFormSubmit(obj)}
                            ref="formRef"
                        />

                        <div className="table-header-block d-flex mt-5 align-items-center">
                            <h5 className="px-2 font-weight-bold table-heading m-0">Trip List</h5>
                        </div>

                        <div className="table-cover table-responsive px-2 mt-4">
                            <table className="table">
                                <thead>
                                    <tr>
                                        <th scope="col"></th>
                                        <th scope="col">No. Order</th>
                                        <th scope="col">Asal Pengiriman</th>
                                        <th scope="col">Tujuan Pengiriman</th>
                                        <th scope="col">FTL/Deskripsi</th>
                                        <th scope="col">Dimensi/Berat</th>
                                        <th scope="col">Kendaraan</th>
                                        <th scope="col">Estimasi Waktu Angkut</th>
                                        <th scope="col">Estimasi Waktu Tiba</th>
                                        <th scope="col">Status</th>

                                    </tr>
                                </thead>
                                <tbody>
                                    {
                                        this.state.roleList.map((x, i) =>
                                            <tr>
                                                <td className={classNames("row-actions d-flex align-items-center", { "border-top-0": !i })}>
                                                    <button type="button" className="btn rounded-circle circular-icon d-flex align-items-center justify-content-center">
                                                        <i className="far fa-trash-alt role-delete"></i>
                                                    </button>
                                                    <button type="button" class="btn btn-primary btn-sm ml-2 px-3 search-button">Reassign</button>
                                                </td>
                                                <td>{x.OrderNo}</td>
                                                <td>{x.Name1}</td>
                                                <td>{x.Name2}</td>
                                                <td>{x.Description}</td>
                                                <td>{x.Dimension}</td>
                                                <td>{x.Vehicle}</td>
                                                <td>{x.EstimatedTransportTime}</td>
                                                <td>{x.EstimatedTransportArrived}</td>
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

export default TripManagement;