package com.example.notpaylasimi;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

public class DatabaseHelper extends SQLiteOpenHelper {

    private static final String VERITABANI_ADI = "notlar.db";
    private static final int VERITABANI_SURUMU = 1;

    private static final String TABLO_NOTLAR = "notlar";
    private static final String KOLON_ID = "id";
    private static final String KOLON_BASLIK = "baslik";
    private static final String KOLON_ICERIK = "icerik";
    private static final String KOLON_TARIH = "tarih";

    public DatabaseHelper(Context context) {
        super(context, VERITABANI_ADI, null, VERITABANI_SURUMU);
    }

    @Override
    public void onCreate(SQLiteDatabase db) {
        String sql = "CREATE TABLE " + TABLO_NOTLAR + " (" +
                KOLON_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                KOLON_BASLIK + " TEXT NOT NULL, " +
                KOLON_ICERIK + " TEXT NOT NULL, " +
                KOLON_TARIH + " TEXT NOT NULL)";
        db.execSQL(sql);
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int eskiSurum, int yeniSurum) {
        db.execSQL("DROP TABLE IF EXISTS " + TABLO_NOTLAR);
        onCreate(db);
    }

    public long notEkle(String baslik, String icerik) {
        SQLiteDatabase db = getWritableDatabase();
        ContentValues degerler = new ContentValues();
        degerler.put(KOLON_BASLIK, baslik);
        degerler.put(KOLON_ICERIK, icerik);
        degerler.put(KOLON_TARIH, simdikiTarih());
        return db.insert(TABLO_NOTLAR, null, degerler);
    }

    public List<Not> tumNotlariGetir() {
        List<Not> notlar = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();
        Cursor cursor = db.query(TABLO_NOTLAR, null, null, null, null, null,
                KOLON_ID + " DESC");

        if (cursor.moveToFirst()) {
            do {
                notlar.add(new Not(
                        cursor.getLong(cursor.getColumnIndexOrThrow(KOLON_ID)),
                        cursor.getString(cursor.getColumnIndexOrThrow(KOLON_BASLIK)),
                        cursor.getString(cursor.getColumnIndexOrThrow(KOLON_ICERIK)),
                        cursor.getString(cursor.getColumnIndexOrThrow(KOLON_TARIH))
                ));
            } while (cursor.moveToNext());
        }
        cursor.close();
        return notlar;
    }

    public void notSil(long id) {
        SQLiteDatabase db = getWritableDatabase();
        db.delete(TABLO_NOTLAR, KOLON_ID + " = ?", new String[]{String.valueOf(id)});
    }

    private String simdikiTarih() {
        SimpleDateFormat format = new SimpleDateFormat("dd.MM.yyyy HH:mm", Locale.getDefault());
        return format.format(new Date());
    }
}
