@@ .. @@
     app.UseHttpsRedirection();
     app.MapDefaultEndpoints();
     app.MapProjectEndpoints();
-    app.MapPostEndpoints();
+    app.MapPostEndpoints();
    app.MapMarketingPlanEndpoints();
    app.MapCalendarEndpoints();
    app.MapAnalyticsEndpoints();
    app.MapNetworkEndpoints();
 
     app.Run();