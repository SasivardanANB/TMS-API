import React from "react";
import { BrowserRouter as Router, Route, Switch } from "react-router-dom";

import { Provider } from 'react-redux';
import { createStore } from 'redux';
import reducer from './store/reducer';

import Layout from './components/common/layout';
import Loader from './components/common/loader';

import routes from './routes.json'

const store = createStore(reducer);

export default class App extends React.Component {
    render() {
        return (
            <Provider store={store}>
                <Router>
                    <React.Suspense fallback={<img width="auto" height="100%" src={require("./img/loader.svg")} />}>
                            <Switch>
                                {
                                    routes.routes.map(x => {
                                        return <Route key={x.name} exact={x.exact} path={x.path} component={Layout} />
                                    })
                                }
                            </Switch>
                    </React.Suspense>
                    <Loader />
                </Router>
            </Provider>
        )
    }
}