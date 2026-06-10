package com.example.notpaylasimi;

public class Not {
    private long id;
    private String baslik;
    private String icerik;
    private String tarih;

    public Not(long id, String baslik, String icerik, String tarih) {
        this.id = id;
        this.baslik = baslik;
        this.icerik = icerik;
        this.tarih = tarih;
    }

    public long getId() {
        return id;
    }

    public String getBaslik() {
        return baslik;
    }

    public String getIcerik() {
        return icerik;
    }

    public String getTarih() {
        return tarih;
    }

    public String paylasimMetni() {
        return baslik + "\n\n" + icerik;
    }
}
