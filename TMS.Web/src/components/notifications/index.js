import React from 'react';
import './notifications.scss';
import classNames from 'classnames/bind';


let 
    notifications = [
        {
            ID: 1,
            Status: "Driver Harryanto has finished his trip (AHM JKT - MD)",
            Date: "5 Apr 2019 13:02"
        },
        {
            ID: 2,
            Status: "2 new orders waiting to be done!",
            Date: "5 Apr 2019 13:02"
        },
    ];

class Notifications extends React.Component {
    constructor(props) {
        super(props);
        this.state = { startDate: new Date() }
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

    }

 

    render() {
        return (
            <React.Fragment>
               
                <div className="tabs-wrap">
                    <div className="tabs-header-wrap">
                        <div className="tabs-title d-none d-md-block d-lg-block active">Notification</div>
                        <div className="clearfix"></div>
                    </div>
                    <div className="tabs-content">

                        {
                            notifications.map((x) =>
                                <div key={x.ID} className="row m-0 py-3 d-flex align-items-center">
                                    <div className="col-md-1 col-lg-1">
                                        <div className="circle bg-primary">
                                            <i className="fas fa-bell fa-lg text-light"></i>
                                        </div>
                                    </div>
                                    <div className="col-md-11 col-lg-11">
                                        <p className="m-0">{x.Status}</p>
                                        <sup className="text-secondary">{x.Date}</sup>
                                    </div>
                                </div>
                            )
                        }

                    </div>
                </div>
            </React.Fragment>
        );
    }
}

export default Notifications;