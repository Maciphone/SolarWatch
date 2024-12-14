import React, { useEffect, useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import Authenticate from "../Components/Authenticate";
import GetTokenFromCookie from "../Components/GetTokenFromCookie";
import "./StyleSheet.css";
import { useCookies } from "react-cookie";
import GetClaimsFromToken from "../Components/GetClaimsFromToken";

export default function Update() {
  const { state } = useLocation();
  const { cityId } = useParams();
  const data = state?.data;
  const [cookie] = useCookies(["token"]);
  const [loading, setLoading] = useState(true);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const navigate = useNavigate();
  if (data) {
    console.log(data);
  }

  const [sunriseSunset, setSunriseSunset] = useState({
    sunriseSunsetId: "",
    sunrise: "",
    sunset: "",
    cityId: "",
  });
  const [cityData, setCityData] = useState({
    cityId: "",
    name: "",
    latitude: "",
    longitude: "",
    state: "",
    country: "",
  });

  const resetCityData = {
    cityId: "",
    name: "",
    latitude: "",
    longitude: "",
    state: "",
    country: "",
  };

  const resetSunriseSunset = {
    sunriseSunsetId: "",
    sunrise: "",
    sunset: "",
    cityId: "",
  };

  useEffect(() => {
    const fetchCityData = async () => {
      var token = GetTokenFromCookie();
      try {
        if (!data || !data.city) {
          throw new Error("Invalid city data: city name is missing");
        }
        console.log(data.city);
        const response = await fetch(
          `/api/SolarWatch/GetCity?cityStr=${data.city}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${cookie.token}`,
            },
            // credentials: "include",
          }
        );
        if (!response.ok) {
          throw new Error("Failed to fetch");
        }

        const cityFetch = await response.json();
        console.log("cityfetch");
        console.log(cityFetch);
        setCityData(cityFetch);
      } catch (error) {
        console.log("catch ágban vagyok");
      }
    };
    fetchCityData();
  }, [data]);

  useEffect(() => {
    const token = GetTokenFromCookie();
    if (token) {
      setIsLoggedIn(true);
    } else {
      setIsLoggedIn(false);
      setLoading(false);
    }
  }, [cookie]);

  useEffect(() => {
    const fetchSunsetSunriseData = async () => {
      var token = GetTokenFromCookie();
      try {
        if (!data || !data.city) {
          throw new Error("Invalid date data:solardata missing");
        }
        console.log(data.city);
        console.log(data.sunrise);
        const response = await fetch(
          `/api/SolarWatch/GetSunriseSunset?cityName=${data.city}&sunrise=${data.sunrise}&date=${data.year}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
            //credentials: "include", // cookie törlődik emiatt?
          }
        );
        if (!response.ok) {
          console.log(response);
          throw new Error("Failed to fetch");
        }
        const datas = await response.json();
        setLoading(false);
        console.log(datas);
        setSunriseSunset(datas);
      } catch (error) {
        console.log("Error: catch ágban vagyok");
      }
    };
    fetchSunsetSunriseData();
  }, [data, cookie]);

  const handleCityChange = (e) => {
    setCityData({
      ...cityData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSunriseSunsetChange = (e) => {
    setSunriseSunset({
      ...sunriseSunset,
      [e.target.name]: e.target.value,
    });
  };

  const handleCitySubmit = async (e) => {
    e.preventDefault();
    var token = GetTokenFromCookie();
    try {
      const response = await fetch(
        `/api/SolarWatch/UpdateCity?id=${cityData.cityId}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${cookie.token}`,
          },
          // credentials: "include", // cookiek törlődnek emiatt??
          body: JSON.stringify(cityData),
        }
      );
      if (!response.ok) {
        throw new Error("Failed to update city data");
      }
      console.log("City data updated successfully");
      alert("update: successful");
    } catch (error) {
      console.log("Error updating city data:", error);
    }
  };

  const handleSunriseSunsetSubmit = async (e) => {
    e.preventDefault();
    var token = GetTokenFromCookie();
    console.log(GetClaimsFromToken(token).role);
    if (token) {
      try {
        const response = await fetch(
          `/api/SolarWatch/UpdateSunsetSunrise?id=${sunriseSunset.sunriseSunsetId}`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${cookie.token}`,
            },
            //   credentials: "include",
            body: JSON.stringify(sunriseSunset),
          }
        );
        if (!response.ok) {
          throw new Error("Failed to update sunrise/sunset data");
        }
        alert("update: successful");
        console.log("Sunrise/Sunset data updated successfully");
      } catch (error) {
        console.log("Error updating sunrise/sunset data:", error);
      }
    } else {
      console.log("logedout");
    }
  };

  const deleteSunsetSunrise = async () => {
    const id = sunriseSunset.sunriseSunsetId;
    try {
      const token = GetTokenFromCookie();
      const response = await fetch(
        `/api/SolarWatch/DeleteSunriseSunset?id=${id}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${cookie.token}`,
          },
          // credentials: "include",
        }
      );

      if (!response.ok) {
        const errorMessage = await response.text();
        alert(`Could not delete: ${errorMessage}`);
        return;
      }

      alert("Sunrise successfully deleted");
      setSunriseSunset(resetSunriseSunset);
    } catch (error) {
      console.error("Error deleting city:", error);
      alert("An error occurred while trying to delete the city.");
    }
  };

  const deleteCity = async () => {
    const token = GetTokenFromCookie();
    try {
      // Aszinkron módon várjuk meg a fetch hívás végét
      const response = await fetch(`/api/SolarWatch/DeleteCity?id=${cityId}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        credentials: "include",
      });

      if (!response.ok) {
        const errorMessage = await response.text();
        alert(`Could not delete: ${errorMessage}`);
        return;
      }

      alert("City successfully deleted");
      setCityData(resetCityData);
      //cascade delete removes all sunrises!
      setSunriseSunset(resetSunriseSunset);
    } catch (error) {
      console.error("Error deleting city:", error);
      alert("An error occurred while trying to delete the city.");
    }
  };

  const goToLogin = () => {
    navigate(`/login`);
  };
  if (loading) {
    return <div>loading...</div>;
  }

  return (
    <div>
      {isLoggedIn ? (
        <div>
          <h2>Update City - ID: {cityId}</h2>
          <form onSubmit={handleCitySubmit}>
            <h3>City Data</h3>
            <label>
              Name:
              <input
                type="text"
                name="name"
                value={cityData.name}
                onChange={handleCityChange}
              />
            </label>
            <label>
              Latitude:
              <input
                type="text"
                name="latitude"
                value={cityData.latitude}
                onChange={handleCityChange}
              />
            </label>
            <label>
              Longitude:
              <input
                type="text"
                name="longitude"
                value={cityData.longitude}
                onChange={handleCityChange}
              />
            </label>
            <label>
              Country:
              <input
                type="text"
                name="country"
                value={cityData.country}
                onChange={handleCityChange}
              />
            </label>
            <button type="submit">Update City</button>
          </form>
          <button onClick={deleteCity}>Delete</button>

          <form onSubmit={handleSunriseSunsetSubmit}>
            <h3>Sunrise/Sunset Data</h3>
            <label>
              Sunrise:
              <input
                type="text"
                name="sunrise"
                value={sunriseSunset.sunrise}
                onChange={handleSunriseSunsetChange}
              />
            </label>
            <label>
              Sunset:
              <input
                type="text"
                name="sunset"
                value={sunriseSunset.sunset}
                onChange={handleSunriseSunsetChange}
              />
            </label>
            <button type="submit">Update Sunrise/Sunset</button>
          </form>
          <button onClick={deleteSunsetSunrise}>Delete</button>
        </div>
      ) : (
        <div>
          <p>You are not logged in.</p>
          <button onClick={goToLogin}>Go to Login</button>
        </div>
      )}
    </div>
  );
}
