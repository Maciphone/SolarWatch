import React, { useState, useEffect } from "react";
import GetRoleFromToken from "../Components/GetRoleFromToken";
import GetTokenFromCookie from "../Components/GetTokenFromCookie";
import { useNavigate } from "react-router-dom";
import "./StyleSheet.css";
import { useCookies } from "react-cookie";

export default function All() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [solarWatchData, setSolarWatchData] = useState([]);
  const [error, setError] = useState(null);
  const [upDateData, setUpdateData] = useState("");
  const [selectedIndex, setSelectedIndex] = useState("");
  const [role, setRole] = useState([]);
  const [cookies] = useCookies(["token"]);
  const navigate = useNavigate();

  useEffect(() => {
    if (isLoggedIn) {
      const fetchSolarWatchData = async () => {
        try {
          const response = await fetch("api/SolarWatch/GetAll", {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer  ${cookies.token}`,
            },
          });

          if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
          }

          const data = await response.json();
          setSolarWatchData(data);
        } catch (error) {
          setError(error.message);
        }
      };
      fetchSolarWatchData();
    }
  }, [isLoggedIn, cookies]);

  useEffect(() => {
    var token = GetTokenFromCookie();
    if (token) {
      var roleLocal = GetRoleFromToken(token);
      setRole(roleLocal);
      setIsLoggedIn(true);
      console.log(roleLocal);
    } else {
      setIsLoggedIn(false);
    }
  }, []);

  if (error) {
    return <div>Error: {error}</div>;
  }

  const handleClick = (item, index) => {
    console.log(item);
    setUpdateData(item);
    setSelectedIndex(index);
  };

  const handleUpdate = () => {
    console.log("Updating item:", upDateData);
    navigate(`/update/${upDateData.cityId}`, { state: { data: upDateData } });
  };

  const goToLogin = () => {
    navigate(`/login`);
  };
  return (
    <div>
      <h2>Solar Watch Data</h2>
      {isLoggedIn ? (
        <ul>
          {solarWatchData.map((item, index) => (
            <li onClick={() => handleClick(item, index)} key={index}>
              <div>Date: {item.year}</div>
              <div>Sunrise: {item.sunrise}</div>
              <div>Sunset: {item.sunset}</div>
              <div>City: {item.city}</div>
              {upDateData && selectedIndex == index && role == "Admin" && (
                <div>
                  <button onClick={handleUpdate}>update</button>
                </div>
              )}
            </li>
          ))}
        </ul>
      ) : (
        <div>
          <p>You are not logged in.</p>
          <button onClick={goToLogin}>Go to Login</button>
        </div>
      )}
    </div>
  );
}
