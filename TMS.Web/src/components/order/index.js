import React from 'react';
import './order.scss';
import CustomBox from '../common/custombox';

const renderBox = [
    {
        name: 'Order',
        circleBgColor: 'bg-primary',
        logo: 'fas fa-box',
        URL: '/order/maintainorder'
    },
    {
        name: 'Packing Sheet',
        circleBgColor: 'bg-success',
        logo: 'fas fa-box-open',
        URL: ''
    },
    
]

class Order extends React.Component {

    render() {
        console.log(this.props);
        return (
            <div className="row shadow-sm bg-light m-0 p-3">

                {
                    renderBox.map((x) =>
                        <div key={x.name} className="col-12 col-md-4 col-lg-4">
                            <CustomBox {...x} onClick={x => this.props.history.push(x)} />
                        </div>
                    )
                }
            </div>

        );
    }
}

export default Order;