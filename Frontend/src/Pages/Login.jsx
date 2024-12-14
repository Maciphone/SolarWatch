//import React, { useState } from "react";

import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCookies } from "react-cookie";
import GetTokenFromCookie from "../Components/GetTokenFromCookie";
import GetRoleFromToken from "../Components/GetRoleFromToken";
import "./StyleSheet.css";

const Login = ({ onLogin }) => {
  const [cookies, setCookie, removeCookie] = useCookies(["token"]);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch("api/auth/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password }),
      });
      if (response.ok) {
        const data = await response.json();
        //document.cookie
        document.cookie = `token=${data.token}; path=/; max-age=30`;
        //cookieProvider
        setCookie("token", data.token, { path: "/", maxAge: 30 });
        onLogin(data.userName, email);
        navigate("/");
      } else {
        setError("Invalid credentials");
      }
    } catch (error) {
      setError("An error occurred");
    }
  };

  const logOut = () => {
    document.cookie = "token=; path=/; max-age=0";
    removeCookie("token");
    navigate("/");
  };

  useEffect(() => {
    if (error) {
      alert(error);
      setError("");
    }
  }, [error]);

  return (
    <div>
      <h2>Login</h2>
      <form onSubmit={handleLogin}>
        <div>
          <label>Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>
        <div>
          <label>Password:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        <button type="submit">Login</button>
      </form>
      <div>
        <button onClick={logOut}>Log out</button>
      </div>
    </div>
  );
};

export default Login;
