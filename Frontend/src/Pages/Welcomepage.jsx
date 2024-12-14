import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import GetTokenFromCookie from "../Components/GetTokenFromCookie";
import GetClaimsFromToken from "../Components/GetClaimsFromToken";
import "./StyleSheet.css";
import { useCookies } from "react-cookie";

const Welcomepage = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [role, setRole] = useState("");
  const [email, setEmail] = useState("");
  const [userName, setUserName] = useState("");
  const [solarWatchData, setSolarWatchData] = useState(null);
  const [cookie] = useCookies(["token"]);
  const [formData, setFormData] = useState({
    city: "",
    date: "",
  });

  useEffect(() => {
    const token = GetTokenFromCookie();
    if (token) {
      const roleToken = GetClaimsFromToken(token).role;
      const emailToken = GetClaimsFromToken(token).email;
      const userNameToken = GetClaimsFromToken(token).name;
      setRole(roleToken);
      setEmail(emailToken);
      setUserName(userNameToken);
      setIsLoggedIn(true);
      console.log(emailToken);
    } else {
      setIsLoggedIn(false);
    }
  }, [cookie]);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log("Form data submitted:", formData);
    fetchSolarWatchData();
  };

  const fetchSolarWatchData = async () => {
    var token = GetTokenFromCookie();
    try {
      console.log("fetch indul");
      const response = await fetch(
        `/api/SolarWatch/GetByCityDateAsyncDatabase?date=${formData.date}&city=${formData.city}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + token,
          },
          credentials: "include",
        }
      );
      if (!response.ok) {
        alert("sorry, something went ");
      }
      var result = await response.json();
      console.log(result);
      setSolarWatchData(result);
    } catch (error) {
      console.error(error);
    }
  };

  const handleBack = () => {
    const resetData = {
      city: "",
      date: "",
    };
    setFormData(resetData);
    setSolarWatchData(null);
  };

  return (
    <div>
      <h1>Welcome!</h1>
      {isLoggedIn ? (
        <div>
          <p>Your email: {email}</p>
          <p>UserName: {userName}</p>
          <div>
            <h2>Enter City and Date</h2>
            <form onSubmit={handleSubmit}>
              <div>
                <label>
                  City:
                  <input
                    type="text"
                    name="city"
                    value={formData.city}
                    onChange={handleChange}
                    required
                  />
                </label>
              </div>
              <div>
                <label>
                  Date:
                  <input
                    type="date"
                    name="date"
                    value={formData.date}
                    onChange={handleChange}
                  />
                </label>
              </div>
              <button type="submit">Submit</button>
            </form>
          </div>
          {solarWatchData && (
            <div>
              <div>Date: {solarWatchData.year}</div>
              <div>Sunrise: {solarWatchData.sunrise}</div>
              <div>Sunset: {solarWatchData.sunset}</div>
              <div>City: {solarWatchData.city}</div>
              <button onClick={handleBack}>Reset</button>
            </div>
          )}
        </div>
      ) : (
        <div>
          <p>Please log in to see your email.</p>
          <Link to={"/login"}>
            <button>Login</button>
          </Link>
        </div>
      )}
    </div>
  );
};

export default Welcomepage;
