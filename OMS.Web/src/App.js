import React from "react";
import { BrowserRouter as Router, Route, Switch } from "react-router-dom";

import { Provider } from 'react-redux';
import { createStore, applyMiddleware } from 'redux';
import thunk from 'redux-thunk';
import rootReducer from './store/reducer';

import Layout from './components/common/layout';
import Loader, { StaticLoader } from './components/common/loader';
import Error from './components/common/error';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

import routes from './routes.json'

const store = createStore(rootReducer, applyMiddleware(thunk));

export default class App extends React.Component {
    render() {
        return (
            <Provider store={store}>
                <Router>
                    <React.Suspense fallback={StaticLoader}>
                            <Switch>
                                {
                                    routes.routes.map(x => {
                                        return <Route key={x.name} exact={x.exact} path={x.path} component={Layout} />
                                    })
                                }
                            </Switch>
                    </React.Suspense>
                    <Loader />
                    <Error />                    
                    <ToastContainer />
                </Router>
            </Provider>
        )
    }
}