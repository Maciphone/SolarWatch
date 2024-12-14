import React from "react";
import { useCookies } from "react-cookie";
import { Link } from "react-router-dom";
import GetRoleFromToken from "./GetRoleFromToken";
import GetTokenFromCookie from "./GetTokenFromCookie";

const Navbar = () => {
  const [cookie] = useCookies(["token"]);
  const token = GetTokenFromCookie();
  let userRole = null;

  if (token) {
    console.log(token);
    userRole = GetRoleFromToken(token);
  }

  return (
    <nav>
      <Link to={"/login"}>
        <button>Login</button>
      </Link>
      <Link to={"/"}>
        <button>Welcomepage</button>
      </Link>
      {userRole === "Admin" && (
        <Link to={"/all"}>
          <button>All</button>
        </Link>
      )}
      <Link to={"/allAuthenticated"}>
        <button>AllAuthenticated</button>
      </Link>
      <Link to={"/allAdmin"}>
        <button>AllAdmin</button>
      </Link>
      <Link to={"/registration"}>
        <button>Registration</button>
      </Link>
    </nav>
  );
};

export default Navbar;
