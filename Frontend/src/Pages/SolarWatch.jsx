import React, { useEffect, useState } from "react";
import Authenticate from "../Components/Authenticate";
import "./StyleSheet.css";

export default function SolarWatch() {
  const [date, setDate] = useState("");
  const [city, setCity] = useState("");
  const [solarWatch, setSolarWatch] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log(date);
    const fetchData = async () => {
      try {
        //az Authenticate returns response.json(); handles issues
        const data = await Authenticate(
          `api/SolarWatch/GetByCityDateAsyncDatabase?date=${date}&city=${city}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
            },
          }
        );
        console.log(data);
        setSolarWatch(data);
      } catch (error) {
        console.error(error);
      }
    };
    fetchData();
  };

  return (
    <div>
      <div>
        <form onSubmit={handleSubmit}>
          <div>
            <label>Date</label>
            <input
              type="date"
              value={date}
              onChange={(e) => setDate(e.target.value)}
            />
          </div>
          <div>
            <label>City</label>
            <input
              type="text"
              value={city}
              onChange={(e) => setCity(e.target.value)}
            />
          </div>
          <button type="submit">Search</button>
        </form>
      </div>

      {solarWatch && (
        <div>
          <div>Date: {solarWatch.year}</div>
          <div>Sunrise: {solarWatch.sunrise}</div>
          <div>Sunset: {solarWatch.sunset}</div>
          <div>City: {solarWatch.city}</div>
        </div>
      )}
    </div>
  );
}
