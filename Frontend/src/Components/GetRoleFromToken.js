import { jwtDecode } from "jwt-decode";


const GetRoleFromToken = (token) => {
    try {
        const decoded = jwtDecode(token);
        const roleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        console.log(decoded[roleClaim]);
        return decoded[roleClaim]; // vagy amit a szerver visszaad a tokenben
    } catch (error) {
        console.error("Failed to decode token", error);
        return null;
    }
};

export default GetRoleFromToken;
