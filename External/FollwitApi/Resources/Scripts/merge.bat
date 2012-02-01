@echo off
md merged
ilmerge /out:merged\FollwitApi.dll FollwitApi.dll CookComputing.XmlRpcV2.dll
