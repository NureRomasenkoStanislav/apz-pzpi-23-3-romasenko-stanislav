from locust import HttpUser, task, constant
import urllib3

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

class RoomBookUser(HttpUser):
    wait_time = constant(0)

    @task
    def get_rooms(self):
        with self.client.get("/api/rooms", verify=False, catch_response=True) as response:
            if response.status_code == 200:
                response.success()
            elif response.status_code == 401:
                response.failure("Помилка 401: Ендпоінт вимагає JWT-токен")
            else:
                response.failure(f"Сервер повернув код: {response.status_code}")