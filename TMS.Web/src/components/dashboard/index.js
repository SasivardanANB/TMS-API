import React from 'react';
import './dashboard.scss';
import classNames from 'classnames/bind';

let DashboardBox = [
    {
        ID: 1,
        Name: "ALL ORDER",
        Value: "20",
        BgClr: "bg-info"
    },
    {
        ID: 2,
        Name: "BOOKED",
        Value: "5",
        BgClr: "bg-primary"
    },
    {
        ID: 3,
        Name: "PICKUP",
        Value: "1",
        BgClr: "bg-primary"
    },
    {
        ID: 4,
        Name: "LOADING",
        Value: "1",
        BgClr: "bg-primary"
    },
    {
        ID: 5,
        Name: "CONFIRMED",
        Value: "3",
        BgClr: "bg-primary"
    },
    {
        ID: 6,
        Name: "ACCEPTED",
        Value: "2",
        BgClr: "bg-primary"
    },
    {
        ID: 7,
        Name: "UNLODING",
        Value: "2",
        BgClr: "bg-primary"
    },
    {
        ID: 8,
        Name: "DROP OFF",
        Value: "1",
        BgClr: "bg-primary"
    },
    {
        ID: 9,
        Name: "POD",
        Value: "1",
        BgClr: "bg-primary"
    },
    {
        ID: 10,
        Name: "CANCELED",
        Value: "4",
        BgClr: "bg-danger"
    }
];

class Dashboard extends React.Component {
    render() {
        console.log(this.props);
        return (
            <div className="dashboard">

            <div className="row">
                <div className="col-md-6 col-lg-6">

                    <div className="row">

                        {
                            DashboardBox.map((x) =>
                                <div key={x.ID} className="col-md-6 col-lg-6">
                                    <div className={classNames("border rounded-lg d-flex p-4 align-items-center text-light m-3 shadow-sm", x.BgClr)}>
                                        <p className="m-0 flex-fill">{x.Name}</p>
                                        <h2 className="m-0"><strong>{x.Value}</strong></h2>
                                    </div>
                                </div>
                            )
                        }

                    </div>

                </div>

                <div className="col-md-6 col-lg-6">
                
                </div>
            </div>

            </div>

        );
    }
}

export default Dashboard;