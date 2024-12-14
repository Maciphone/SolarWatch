import React from "react";
import { Navigate } from "react-router-dom";
import GetTokenFromCookie from "./GetTokenFromCookie";
import GetRoleFromToken from "./GetRoleFromToken";

const RouteWithRole = ({ children, role }) => {
  const token = GetTokenFromCookie();

  if (!token) {
    return <Navigate to="/login" />;
  }

  const userRole = GetRoleFromToken(token);

  if (userRole !== role) {
    return <Navigate to="/" />;
  }

  return children;
};

export default RouteWithRole;
