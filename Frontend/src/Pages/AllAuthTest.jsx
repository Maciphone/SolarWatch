import { useEffect, useState } from "react";
import Authenticate from "../Components/Authenticate";
import "./StyleSheet.css";
import { useCookies } from "react-cookie";

export default function AllAuthTest() {
  const [solarWatchData, setSolarWatchData] = useState([]);
  const [error, setError] = useState(null);
  const [cookies] = useCookies(["token"]);

  useEffect(() => {
    const token = cookies.token;
    const fetchSolarWatchData = async () => {
      try {
        console.log("fetch indul");
        const data = await Authenticate("api/SolarWatch/GetAll", {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          credentials: "include",
        });

        setSolarWatchData(data);
      } catch (error) {
        setError(error.message);
      }
    };

    if (token) {
      fetchSolarWatchData();
    }
  }, [cookies]);

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div>
      <h2>Solar Watch Data</h2>
      <ul>
        {solarWatchData.map((item, index) => (
          <li key={index}>
            <div>Year: {item.year}</div>
            <div>Sunrise: {item.sunrise}</div>
            <div>Sunset: {item.sunset}</div>
            <div>City: {item.city}</div>
          </li>
        ))}
      </ul>
    </div>
  );
}
