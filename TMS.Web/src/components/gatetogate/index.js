import React from 'react';
import './gatetogate.scss';
import classNames from 'classnames/bind';

import ToggleBound from '../common/togglebound';
import Form from '../common/form';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

let 
    uploadFormElems = [
        {
            name: 'Search Gate',
            placeholder: 'Search Gate',
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
            Gate: "-",
            Police: "B 123 AA",
            Unloading: "Bongkar",
            Type: "Truck",
            Status: "NOT ARRIVED",
            Action: "GATE IN",
           
        },
        {
            ID: 2,
            Gate: "Gate2",
            Police: "B 123 AA",
            Unloading: "Bongkar",
            Type: "Truck",
            Status: "GATE IN",
            Action: "GATE OUT",
        }
    ];

class Gatetogate extends React.Component {
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
                        <div className="tabs-title d-none d-md-block d-lg-block active">Gate</div>
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

                        <div className="table-header-block d-flex mt-5 align-items-center">
                            <h5 className="px-2 font-weight-bold table-heading m-0">Gate List</h5>
                        </div>

                        <div className="table-cover table-responsive px-2 mt-4">
                            <table className="table">
                                <thead>
                                    <tr>
                                        <th scope="col">Gate</th>
                                        <th scope="col">No. Polisi</th>
                                        <th scope="col">Bongkar/Muat</th>
                                        <th scope="col">Tipe Kendaraan</th>
                                        <th scope="col">Status</th>
                                        <th scope="col">Action</th>

                                    </tr>
                                </thead>
                                <tbody>
                                    {
                                        this.state.roleList.map((x, i) =>
                                            <tr>
                                                <td>{x.Gate}</td>
                                                <td>{x.Police}</td>
                                                <td>{x.Unloading}</td>
                                                <td>{x.Type}</td>
                                                <td>{x.Status}</td>
                                                <td className={classNames("row-actions d-flex align-items-center", { "border-top-0": !i })}>
                                                    <button type="button" class="btn btn-primary btn-sm ml-2 px-3 search-button">{x.Action}</button>
                                                </td>
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

export default Gatetogate;