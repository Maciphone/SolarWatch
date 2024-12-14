import "./App.css";
import Layout from "./Components/Layout";
import Login from "./Pages/Login";
import { useRoutes } from "react-router-dom";

import Welcomepage from "./Pages/Welcomepage";
import { useState } from "react";
import All from "./Pages/All";
import AllAuthTest from "./Pages/AllAuthTest";
import RouteWithRole from "./Components/RouteWithRole";
import Registration from "./Pages/Registration";
import SolarWatch from "./Pages/SolarWatch";
import Update from "./Pages/Update";
import { CookiesProvider } from "react-cookie";

const App = () => {
  const [userName, setUserName] = useState(null);
  const [email, setEmail] = useState(null);

  const handleLogin = (name, mail) => {
    setUserName(name);
    setEmail(mail);
  };
  const routes = useRoutes([
    {
      path: "/",
      element: <Layout userName={userName} email={email} />,
      children: [
        {
          path: "/",
          element: <Welcomepage />,
        },
        {
          path: "/login",
          element: <Login onLogin={handleLogin} />,
        },
        {
          path: "/all",
          element: <All />,
        },
        {
          path: "/allAuthenticated",
          element: <AllAuthTest />,
        },
        {
          path: "/allAdmin",
          element: (
            <RouteWithRole role="Admin">
              <AllAuthTest />
            </RouteWithRole>
          ),
        },
        {
          path: "/registration",
          element: <Registration />,
        },
        {
          path: "/solarwatch",
          element: <SolarWatch />,
        },
        {
          path: "/update/:cityId",
          element: <Update />,
        },
      ],
    },
  ]);
  return <CookiesProvider>{routes}</CookiesProvider>;
};
export default App;
