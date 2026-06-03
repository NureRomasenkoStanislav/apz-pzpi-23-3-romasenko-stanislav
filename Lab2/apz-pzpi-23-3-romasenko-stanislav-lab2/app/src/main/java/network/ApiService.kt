package com.example.roombookmobile.network

import com.example.roombookmobile.models.*
import retrofit2.Call
import retrofit2.http.*

interface ApiService {
    @POST("api/auth/login")
    fun login(@Body request: LoginRequest): Call<LoginResponse>

    @GET("api/rooms")
    fun getRooms(@Header("Authorization") token: String): Call<List<RoomDto>>

    @POST("api/bookings")
    fun createBooking(@Header("Authorization") token: String, @Body booking: BookingDto): Call<BookingDto>

    @POST("api/rooms")
    fun createRoom(@Header("Authorization") token: String, @Body room: RoomDto): Call<RoomDto>

    @PUT("api/rooms/{id}")
    fun updateRoom(@Header("Authorization") token: String, @Path("id") id: Int, @Body room: RoomDto): Call<RoomDto>

    @DELETE("api/rooms/{id}")
    fun deleteRoom(@Header("Authorization") token: String, @Path("id") id: Int): Call<Void>
}